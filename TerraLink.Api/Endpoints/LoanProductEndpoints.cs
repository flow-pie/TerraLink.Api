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

        return app;
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