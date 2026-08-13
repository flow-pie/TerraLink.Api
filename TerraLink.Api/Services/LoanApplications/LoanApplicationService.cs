

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
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
                c => c.User!.Id == clientId, //Im the only one who knows how this works
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
            ClientId = client.Id, //a little bit confusing but,, yeah
            RequestedAmount = request.RequestedAmount,
            DurationMonths = request.DurationMonths,
            Purpose = request.Purpose,
            Status = LoanApplicationStatus.SUBMITTED
        };

        dbContext.LoanApplications.Add(application);

        // Generates application.Id
        await dbContext.SaveChangesAsync(cancellationToken);

        application.ApplicationNo =
            GenerateApplicationNumber(application.Id);

        // Save ApplicationNo
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

    private static string GenerateApplicationNumber(long applicationId)
    {
        var suffix = Guid.NewGuid()
            .ToString("N")[..2]
            .ToUpperInvariant();

        return $"TL-{applicationId:D5}-{suffix}";
    }
}
