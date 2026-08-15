using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.LoanApplications;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.LoanApplications;

public class LoanApplicationService(
    TerraLinkDbContext dbContext
) : ILoanApplicationService
{
    public async Task<LoanApplicationResponse?> CreateLoanApplicationAsync(
        long clientId,
        CreateLoanApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var client = await dbContext.Clients
            .SingleOrDefaultAsync(
                c => c.User!.Id == clientId,
                cancellationToken);

        if (client is null)
            return null;

        if (client.VerificationStatus != VerificationStatus.VERIFIED)
        {
            throw new ForbidenException(
                "Client must be verified before applying for a loan.");
        }

        var product = await dbContext.LoanProducts
            .SingleOrDefaultAsync(
                p => p.Id == request.LoanProductId,
                cancellationToken);

        if (product is null)
            return null;

        if (product.Status != LoanProductStatus.ACTIVE)
        {
            throw new ValidationException(
                "The selected loan product is not active.");
        }

        if (request.RequestedAmount < product.MinimumAmount ||
            request.RequestedAmount > product.MaximumAmount)
        {
            throw new ValidationException(
                $"Requested amount must be between " +
                $"{product.MinimumAmount} and {product.MaximumAmount}.");
        }

        if (request.DurationMonths < product.MinimumDuration ||
            request.DurationMonths > product.MaximumDuration)
        {
            throw new ValidationException(
                $"Duration must be between " +
                $"{product.MinimumDuration} and {product.MaximumDuration} months.");
        }

        var application = new LoanApplication
        {
            LoanProductId = request.LoanProductId,
            ClientId = client.Id,
            RequestedAmount = request.RequestedAmount,
            DurationMonths = request.DurationMonths,
            Purpose = request.Purpose,
            Status = LoanApplicationStatus.SUBMITTED
        };

        dbContext.LoanApplications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);

        application.ApplicationNo = GenerateApplicationNumber(application.Id);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new LoanApplicationResponse(
            application.Id,
            application.ApplicationNo,
            application.ClientId,
            application.LoanProductId,
            application.RequestedAmount,
            application.DurationMonths,
            application.Purpose,
            application.Status,
            application.SubmittedAt
        );
    }

    public async Task<PagedResponse<LoanApplicationListItemResponse>> GetLoanApplicationsAsync(
        GetLoanApplicationsRequest request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.LoanApplications
            .AsNoTracking()
            .Include(a => a.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<LoanApplicationStatus>(request.Status, true, out var status))
        {
            query = query.Where(a => a.Status == status);
        }

        if (request.ClientId.HasValue)
        {
            query = query.Where(a => a.ClientId == request.ClientId.Value);
        }

        query = query.OrderByDescending(a => a.SubmittedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new LoanApplicationListItemResponse(
                a.Id,
                a.ApplicationNo!,
                a.Client.FullName,
                a.RequestedAmount,
                a.Status,
                a.SubmittedAt
            ))
            .ToListAsync(cancellationToken);

        return new PagedResponse<LoanApplicationListItemResponse>(
            items,
            request.Page,
            request.PageSize,
            totalCount
        );
    }

    public async Task<LoanApplicationDetailResponse?> GetLoanApplicationDetailAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var application = await dbContext.LoanApplications
            .AsNoTracking()
            .Include(a => a.Client)
            .Include(a => a.LoanProduct)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (application is null)
            return null;

        var creditScore = await GetOrComputeCreditScoreAsync(application.ClientId, dbContext, cancellationToken);

        var creditHistory = await dbContext.CreditHistories
            .AsNoTracking()
            .Include(ch => ch.Loan)
            .Where(ch => ch.ClientId == application.ClientId)
            .OrderByDescending(ch => ch.Id)
            .Select(ch => new CreditHistoryDetail(
                ch.Loan.LoanNo,
                ch.CreditScore,
                ch.RepaymentRating,
                ch.Loan.Status
            ))
            .ToListAsync(cancellationToken);

        var incomeAssessment = await dbContext.IncomeAssessments
            .AsNoTracking()
            .Where(ia => ia.LoanApplicationId == id)
            .OrderByDescending(ia => ia.AssessedAt)
            .Select(ia => new IncomeAssessmentDetail(
                ia.BusinessRevenue,
                ia.OtherIncome,
                ia.HouseholdExpenses,
                ia.DisposableIncome
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (incomeAssessment is null)
        {
            incomeAssessment = new IncomeAssessmentDetail(0, 0, 0, 0);
        }

        var assets = await dbContext.Set<ClientAsset>()
            .AsNoTracking()
            .Where(ca => ca.ClientId == application.ClientId)
            .Select(ca => new AssetDetail(
                ca.AssetType,
                ca.Description,
                ca.Quantity
            ))
            .ToListAsync(cancellationToken);

        return new LoanApplicationDetailResponse(
            new ApplicationDetail(
                application.Id,
                application.ApplicationNo!,
                application.RequestedAmount,
                application.DurationMonths,
                application.Purpose,
                application.Status
            ),
            new ClientDetail(
                application.Client.Id,
                application.Client.FullName,
                application.Client.ClientNo
            ),
            creditScore,
            creditHistory,
            incomeAssessment,
            assets
        );
    }

    public async Task<AppraiseLoanApplicationResponse?> AppraiseLoanApplicationAsync(
        long id,
        AppraiseLoanApplicationRequest request,
        long officerId,
        CancellationToken cancellationToken)
    {
        if (request.Decision is not LoanDecision.APPROVED and not LoanDecision.REJECTED and not LoanDecision.INFO_REQUESTED)
        {
            throw new ValidationException("Decision must be APPROVED, REJECTED, or INFO_REQUESTED.");
        }

        var application = await dbContext.LoanApplications
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (application is null)
            return null;

        if (application.Decision is not null)
        {
            throw new ConflictException("Application has already been decided.");
        }

        application.Status = request.Decision switch
        {
            LoanDecision.APPROVED => LoanApplicationStatus.APPROVED,
            LoanDecision.REJECTED => LoanApplicationStatus.REJECTED,
            LoanDecision.INFO_REQUESTED => LoanApplicationStatus.INFO_REQUESTED,
            _ => application.Status
        };

        application.Decision = request.Decision;
        application.DecisionNotes = request.DecisionNotes;
        application.CreditScoreSnapshot = request.CreditScoreSnapshot;
        application.DecidedAt = DateTime.UtcNow;
        application.AppraisedBy = officerId;

        long? createdLoanId = null;

        if (request.Decision == LoanDecision.APPROVED)
        {
            var loan = new Loan
            {
                LoanNo = GenerateLoanNumber(),
                ApplicationId = application.Id,
                ClientId = application.ClientId,
                ApprovedAmount = application.RequestedAmount,
                Balance = application.RequestedAmount,
                Status = LoanStatus.PENDING_DISBURSEMENT
            };

            dbContext.Loans.Add(loan);
            await dbContext.SaveChangesAsync(cancellationToken);
            createdLoanId = loan.Id;
        }
        else
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new AppraiseLoanApplicationResponse(
            application.Id,
            application.Status,
            application.Decision,
            application.DecidedAt,
            createdLoanId
        );
    }

    public async Task<LoanApplicationStatusResponse?> GetLoanApplicationStatusAsync(
        long id,
        long userId,
        CancellationToken cancellationToken)
    {
        var application = await dbContext.LoanApplications
            .AsNoTracking()
            .FirstOrDefaultAsync(
                a => a.Id == id &&
                a.Client.UserId == userId,
                cancellationToken
                );

        if (application is null)
            return null;
        Console.WriteLine($"Logged in user: {userId}");
        var assignedOfficer = application.AppraisedByUser is not null
            ? new AssignedOfficer(application.AppraisedByUser.Username ?? application.AppraisedByUser.Email ?? "Unknown")
            : null;

        var timeline = GetStatusTimeline(application.Status);

        return new LoanApplicationStatusResponse(
            application.Status,
            timeline,
            assignedOfficer
        );
    }

    public async Task<bool> IsClientOwnerAsync(
        long clientId,
        long userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Clients
            .AnyAsync(c => c.Id == clientId && c.UserId == userId, cancellationToken);
    }

    private static List<StatusTimelineItem> GetStatusTimeline(LoanApplicationStatus status)
    {
        var timeline = new List<StatusTimelineItem>
        {
            new StatusTimelineItem("SUBMITTED", DateTime.UtcNow),
            new StatusTimelineItem("UNDER_REVIEW", null),
            new StatusTimelineItem("APPROVAL", null),
            new StatusTimelineItem("DISBURSEMENT", null)
        };

        var submittedCompleted = DateTime.UtcNow;
        var underReviewCompleted = status is LoanApplicationStatus.UNDER_REVIEW or LoanApplicationStatus.APPROVED or LoanApplicationStatus.REJECTED or LoanApplicationStatus.INFO_REQUESTED
            ? DateTime.UtcNow
            : (DateTime?)null;
        var approvalCompleted = status is LoanApplicationStatus.APPROVED or LoanApplicationStatus.REJECTED
            ? DateTime.UtcNow
            : (DateTime?)null;

        timeline[0] = new StatusTimelineItem("SUBMITTED", submittedCompleted);
        timeline[1] = new StatusTimelineItem("UNDER_REVIEW", underReviewCompleted);
        timeline[2] = new StatusTimelineItem("APPROVAL", approvalCompleted);

        return timeline;
    }

    private static async Task<CreditScoreDetail> GetOrComputeCreditScoreAsync(
        long clientId,
        TerraLinkDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var creditHistory = await dbContext.CreditHistories
            .AsNoTracking()
            .Where(ch => ch.ClientId == clientId)
            .OrderByDescending(ch => ch.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (creditHistory is not null)
        {
            var rating = creditHistory.CreditScore switch
            {
                >= 800 => "A-PRIME",
                >= 700 => "A-PRIME",
                >= 600 => "B-PRIME",
                >= 500 => "C-PRIME",
                _ => "D-PRIME"
            };

            return new CreditScoreDetail(creditHistory.CreditScore, rating);
        }

        return new CreditScoreDetail(650, "C-PRIME");
    }

    private static string GenerateApplicationNumber(long applicationId)
    {
        var suffix = Guid.NewGuid()
            .ToString("N")[..2]
            .ToUpperInvariant();

        return $"TL-{applicationId:D5}-{suffix}";
    }

    private static string GenerateLoanNumber()
    {
        var year = DateTime.UtcNow.Year;
        var suffix = Guid.NewGuid()
            .ToString("N")[..4]
            .ToUpperInvariant();

        return $"MLF-{year}-{suffix}";
    }
}
