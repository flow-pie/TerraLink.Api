using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.LoanApplications;
using TerraLink.Api.Services.LoanApplications;

namespace TerraLink.Api.Endpoints;

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

        group.MapGet("/", GetLoanApplicationsAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{id}", GetLoanApplicationDetailAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer", "Client"));

        group.MapPost("/{id}/appraise", AppraiseLoanApplicationAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{id}/status", GetLoanApplicationStatusAsync)
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

    private static async Task<IResult> GetLoanApplicationsAsync(
        [AsParameters] GetLoanApplicationsRequest request,
        ILoanApplicationService loanApplicationService,
        CancellationToken cancellationToken)
    {
        var result = await loanApplicationService.GetLoanApplicationsAsync(
            request,
            cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> GetLoanApplicationDetailAsync(
        long id,
        ClaimsPrincipal principal,
        ILoanApplicationService loanApplicationService,
        CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();
        var isLoanOfficer = principal.IsInRole("Loan Officer");

        if (!isLoanOfficer)
        {
            var application = await loanApplicationService.GetLoanApplicationDetailAsync(id, cancellationToken);
            if (application is null)
                return Results.NotFound();

            var isOwner = await loanApplicationService.IsClientOwnerAsync(application.Client.Id, loggedInUserId, cancellationToken);
            if (!isOwner)
                return Results.Forbid();
        }

        var result = await loanApplicationService.GetLoanApplicationDetailAsync(id, cancellationToken);
        if (result is null)
            return Results.NotFound();

        return Results.Ok(result);
    }

    private static async Task<IResult> AppraiseLoanApplicationAsync(
        long id,
        AppraiseLoanApplicationRequest request,
        ClaimsPrincipal principal,
        ILoanApplicationService loanApplicationService,
        CancellationToken cancellationToken)
    {
        try
        {
            var officerId = principal.GetUserId();
            var result = await loanApplicationService.AppraiseLoanApplicationAsync(id, request, officerId, cancellationToken);

            if (result is null)
                return Results.NotFound();

            return Results.Ok(result);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, new List<string>()));
        }
        catch (ConflictException ex)
        {
            return Results.Conflict(new { message = ex.Message });
        }
    }

    private static async Task<IResult> GetLoanApplicationStatusAsync(
        long id,
        ClaimsPrincipal principal,
        ILoanApplicationService loanApplicationService,
        CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();
        var result = await loanApplicationService.GetLoanApplicationStatusAsync(id, loggedInUserId, cancellationToken);

        if (result is null)
            return Results.Forbid();

        return Results.Ok(result);
    }
}
