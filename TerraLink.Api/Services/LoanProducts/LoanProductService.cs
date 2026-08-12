using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.LoanProducts;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.LoanProducts;

public class LoanProductService(
    TerraLinkDbContext dbContext
) : ILoanProductService
{
    public async Task<LoanProductResponse> CreateAsync(
        CreateLoanProductRequest request,
        CancellationToken cancellationToken
    )
    {
        if (request.MinimumAmount > request.MaximumAmount)
            throw new ValidationException("Minimum amount cant be greater than maximum amount");
        

        if (request.MinimumDuration > request.MaximumDuration)
            throw new ValidationException("Minimum duration cant be greater than maximum duration");
        

        var loanProductExists = await dbContext.LoanProducts
            .AnyAsync(lp => lp.Name == request.Name,
            cancellationToken
            );

        if(loanProductExists)
            throw new ConflictException("Loan product exists");

        var product =  new LoanProduct{
                Name = request.Name.Trim(),
                MinimumAmount = request.MinimumAmount,
                MaximumAmount = request.MaximumAmount,
                InterestRate = request.InterestRate,
                ProcessingFee = request.ProcessingFee,
                LateFee = request.LateFee,
                MinimumDuration = request.MinimumDuration,
                MaximumDuration = request.MaximumDuration,
                RepaymentFrequency = request.RepaymentFrequency,
                Status = LoanProductStatus.ACTIVE
            };    

        dbContext.LoanProducts.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new LoanProductResponse(
            product.Id,
            product.Name,
            product.MinimumAmount,
            product.MaximumAmount,
            product.InterestRate,
            product.ProcessingFee,
            product.LateFee,
            product.MinimumDuration,
            product.MaximumDuration,
            product.RepaymentFrequency,
            product.Status
        );
    }

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