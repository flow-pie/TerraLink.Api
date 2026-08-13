using System.Security.Claims;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.LoanApplications;
using TerraLink.Api.Services.LoanApplications;

public static class LoanApplicationEndpoints
{
    public static IEndpointRouteBuilder
        MapLoanApplicationEndpoints(
            this IEndpointRouteBuilder app
        )
    {
        var group = app.MapGroup("/api/loan-applications")
            .WithTags("Loan Application");

        group.MapPost("/", CreateLoanApplicationAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Client"));

        return group;

    }

    private static async Task<IResult> CreateLoanApplicationAsync(
    CreateLoanApplicationRequest request,
    ClaimsPrincipal principal,
    ILoanApplicationService loanApplicationService,
    CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();

        var application = await loanApplicationService.CreateLoanApplicationAsync(
            loggedInUserId,
            request,
            cancellationToken);

        if (application is null)
            return Results.NotFound($"User {loggedInUserId} not found");

        return Results.Created(
            $"/api/loan-applications/{application.Id}",
            application);
    }
}