using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.CreditScoring;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.CreditScoring;

public class CreditScoringService(
    TerraLinkDbContext dbContext
) : ICreditScoringService
{
    public async Task<List<CreditHistoryResponse>> GetCreditHistoryAsync(
        long clientId,
        CancellationToken cancellationToken)
    {
        return await dbContext.CreditHistories
            .AsNoTracking()
            .Include(ch => ch.Loan)
            .Where(ch => ch.ClientId == clientId)
            .OrderByDescending(ch => ch.Id)
            .Select(ch => new CreditHistoryResponse(
                ch.Loan.LoanNo,
                ch.CreditScore,
                ch.RepaymentRating
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<AssetResponse>> GetClientAssetsAsync(
        long clientId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Set<ClientAsset>()
            .AsNoTracking()
            .Where(ca => ca.ClientId == clientId)
            .OrderByDescending(ca => ca.RecordedAt)
            .Select(ca => new AssetResponse(
                ca.Id,
                ca.ClientId,
                ca.AssetType,
                ca.Description,
                ca.Quantity,
                ca.RecordedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<AssetResponse?> CreateAssetAsync(
        long clientId,
        CreateAssetRequest request,
        CancellationToken cancellationToken)
    {
        var client = await dbContext.Clients
            .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken);

        if (client is null)
            return null;

        if (!Enum.IsDefined(typeof(AssetType), request.AssetType))
        {
            throw new ValidationException("assetType is not one of LIVESTOCK, MOTORBIKE, WATER_PUMP, OTHER.");
        }

        var asset = new ClientAsset
        {
            ClientId = clientId,
            AssetType = request.AssetType,
            Description = request.Description,
            Quantity = request.Quantity
        };

        dbContext.Set<ClientAsset>().Add(asset);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AssetResponse(
            asset.Id,
            asset.ClientId,
            asset.AssetType,
            asset.Description,
            asset.Quantity,
            asset.RecordedAt
        );
    }

    public async Task<List<IncomeAssessmentResponse>> GetIncomeAssessmentsAsync(
        long clientId,
        CancellationToken cancellationToken)
    {
        return await dbContext.IncomeAssessments
            .AsNoTracking()
            .Where(ia => ia.ClientId == clientId)
            .OrderByDescending(ia => ia.AssessedAt)
            .Select(ia => new IncomeAssessmentResponse(
                ia.Id,
                ia.ClientId,
                ia.LoanApplicationId,
                ia.BusinessRevenue,
                ia.OtherIncome,
                ia.HouseholdExpenses,
                ia.DisposableIncome,
                ia.AssessedAt
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<IncomeAssessmentResponse?> CreateIncomeAssessmentAsync(
        long applicationId,
        CreateIncomeAssessmentRequest request,
        CancellationToken cancellationToken)
    {
        var application = await dbContext.LoanApplications
            .FirstOrDefaultAsync(a => a.Id == applicationId, cancellationToken);

        if (application is null)
            return null;

        var disposableIncome = request.BusinessRevenue + request.OtherIncome - request.HouseholdExpenses;

        var assessment = new IncomeAssessment
        {
            ClientId = application.ClientId,
            LoanApplicationId = applicationId,
            BusinessRevenue = request.BusinessRevenue,
            OtherIncome = request.OtherIncome,
            HouseholdExpenses = request.HouseholdExpenses,
            DisposableIncome = disposableIncome
        };

        dbContext.IncomeAssessments.Add(assessment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new IncomeAssessmentResponse(
            assessment.Id,
            assessment.ClientId,
            assessment.LoanApplicationId,
            assessment.BusinessRevenue,
            assessment.OtherIncome,
            assessment.HouseholdExpenses,
            assessment.DisposableIncome,
            assessment.AssessedAt
        );
    }

    public async Task<CreditScoreResponse?> GetCreditScoreAsync(
        long clientId,
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

            return new CreditScoreResponse(creditHistory.CreditScore, rating);
        }

        return new CreditScoreResponse(650, "C-PRIME");
    }
}
