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

        if (loanProductExists)
            throw new ConflictException("Loan product exists");

        var product = new LoanProduct
        {
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

    public async Task<LoanProductResponse?> UpdateAsync(
    long id,
    UpdateLoanProductRequest request,
    CancellationToken cancellationToken)
    {
        var product = await dbContext.LoanProducts
            .SingleOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);

        if (product is null)
            return null;

        if (request.Name is not null)
        {
            var name = request.Name.Trim();

            var nameExists = await dbContext.LoanProducts
                .AnyAsync(
                    p => p.Id != id &&
                         p.Name == name,
                    cancellationToken);

            if (nameExists)
            {
                throw new ConflictException(
                    "A loan product with that name already exists."
                );
            }

            product.Name = name;
        }

        // Calculate the values after applying the patch.
        // ?? Null coalescing operator
        var minimumAmount =
            request.MinimumAmount ?? product.MinimumAmount;

        var maximumAmount =
            request.MaximumAmount ?? product.MaximumAmount;

        var minimumDuration =
            request.MinimumDuration ?? product.MinimumDuration;

        var maximumDuration =
            request.MaximumDuration ?? product.MaximumDuration;

        if (minimumAmount > maximumAmount)
        {
            throw new ValidationException(
                "Minimum amount cannot be greater than maximum amount."
            );
        }

        if (minimumDuration > maximumDuration)
        {
            throw new ValidationException(
                "Minimum duration cannot be greater than maximum duration."
            );
        }

        if (request.MinimumAmount is not null)
            product.MinimumAmount = request.MinimumAmount.Value;

        if (request.MaximumAmount is not null)
            product.MaximumAmount = request.MaximumAmount.Value;

        if (request.InterestRate is not null)
            product.InterestRate = request.InterestRate.Value;

        if (request.ProcessingFee is not null)
            product.ProcessingFee = request.ProcessingFee.Value;

        if (request.LateFee is not null)
            product.LateFee = request.LateFee.Value;

        if (request.MinimumDuration is not null)
            product.MinimumDuration = request.MinimumDuration.Value;

        if (request.MaximumDuration is not null)
            product.MaximumDuration = request.MaximumDuration.Value;

        if (request.RepaymentFrequency is not null)
            product.RepaymentFrequency =
                request.RepaymentFrequency.Value;

        if (request.Status is not null)
            product.Status = request.Status.Value;

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
}