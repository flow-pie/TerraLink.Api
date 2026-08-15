using System.Security.Claims;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.Loans;
using TerraLink.Api.Services.Loans;

namespace TerraLink.Api.Endpoints;

public static class LoanEndpoints
{
    public static IEndpointRouteBuilder MapLoanEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loans")
            .WithTags("Loans");

        group.MapGet("/", GetLoansAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{id}", GetLoanDetailAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer", "Client"));

        group.MapGet("/clients/{clientId}/loans", GetClientLoansAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer", "Client"));

        return group;
    }

    private static async Task<IResult> GetLoansAsync(
        [AsParameters] GetLoansRequest request,
        ILoanService loanService,
        CancellationToken cancellationToken)
    {
        var result = await loanService.GetLoansAsync(
            request.Status,
            request.Search,
            request.Page,
            request.PageSize,
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetLoanDetailAsync(
        long id,
        ClaimsPrincipal principal,
        ILoanService loanService,
        CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();
        var isLoanOfficer = principal.IsInRole("Loan Officer");

        if (!isLoanOfficer)
        {
            var isOwner = await loanService.IsLoanOwnerAsync(id, loggedInUserId, cancellationToken);
            if (!isOwner)
                return Results.Forbid();
        }

        var result = await loanService.GetLoanDetailAsync(id, cancellationToken);
        if (result is null)
            return Results.NotFound($"Loan with id {id} not found");

        return Results.Ok(result);
    }

    private static async Task<IResult> GetClientLoansAsync(
        long clientId,
        ClaimsPrincipal principal,
        ILoanService loanService,
        CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();
        var isLoanOfficer = principal.IsInRole("Loan Officer");


        if (!isLoanOfficer)
        {
            var isOwner = await loanService.IsLoanOwnerAsync(null, loggedInUserId, cancellationToken);
            if (!isOwner)
                return Results.Forbid();
        }

        var result = await loanService.GetClientLoansAsync(clientId, cancellationToken);
        return Results.Ok(result);
    }
}
