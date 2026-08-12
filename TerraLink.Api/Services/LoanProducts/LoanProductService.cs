using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.LoanProducts;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.LoanProducts;

public class LoanProductService(
    TerraLinkDbContext dbContext
) : ILoanProductService
{
    public async Task<List<LoanProductResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken)
    {
        //build query first
        var query = dbContext.LoanProducts
            .AsNoTracking();
        
        //conditionally modifying the query
        if (!includeInactive)
        {
            query = query.Where(
                p => p.Status == LoanProductStatus.ACTIVE
            );
        }

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new LoanProductResponse(
                p.Id,
                p.Name,
                p.MinimumAmount,
                p.MaximumAmount,
                p.InterestRate,
                p.ProcessingFee,
                p.LateFee,
                p.MinimumDuration,
                p.MaximumDuration,
                p.RepaymentFrequency,
                p.Status
            ))
            .ToListAsync(cancellationToken);
    }
}