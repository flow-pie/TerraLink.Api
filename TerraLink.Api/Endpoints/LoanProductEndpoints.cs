using System.ComponentModel.DataAnnotations;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.LoanProducts;
using TerraLink.Api.Services.LoanProducts;

namespace TerraLink.Api.Endpoints;

public static class LoanProductEndpoints
{
    public static IEndpointRouteBuilder MapLoanProductEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loan-products");

        group.MapGet("/", GetLoanProductsAsync)
            .RequireAuthorization(
                policy => policy.RequireRole(
                    "Loan Officer",
                    "Client"
                ));

        group.MapPost("/", CreateLoanProductAsync)
            .RequireAuthorization(
                policy => policy.RequireRole("Loan Officer"));

        group.MapPatch("/{id:long}", UpdateLoanProductAsync)
            .RequireAuthorization(
                policy => policy.RequireRole("Loan Officer"));

        return app;
    }

    private static async Task<IResult> UpdateLoanProductAsync(
        long id,
        UpdateLoanProductRequest request,
        ILoanProductService loanProductService,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await loanProductService.UpdateAsync(
                id,
                request,
                cancellationToken);

            if (product is null)
                return Results.NotFound(
                    $"Loan product with id {id} not found.");

            return Results.Ok(product);
        }
        catch (ConflictException ex)
        {
            return Results.Conflict(new
            {
                message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    private static async Task<IResult> CreateLoanProductAsync(
        CreateLoanProductRequest request,
        ILoanProductService loanProductService,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await loanProductService.CreateAsync(
                request,
                cancellationToken);

            return Results.Created(
                $"/api/loan-products/{product.Id}",
                product);
        }
        catch (ConflictException ex)
        {
            return Results.Conflict(new
            {
                message = ex.Message
            });
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    private static async Task<IResult> GetLoanProductsAsync(
        bool includeInactive,
        ILoanProductService loanProductService,
        CancellationToken cancellationToken)
    {
        var products = await loanProductService.GetAllAsync(
            includeInactive,
            cancellationToken);

        return Results.Ok(products);
    }
}