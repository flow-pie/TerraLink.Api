using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.CreditScoring;
using TerraLink.Api.Services.CreditScoring;

namespace TerraLink.Api.Endpoints;

public static class CreditScoringEndpoints
{
    public static IEndpointRouteBuilder MapCreditScoringEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/clients")
            .WithTags("Credit Scoring");

        group.MapGet("/{clientId}/credit-history", GetCreditHistoryAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{clientId}/assets", GetClientAssetsAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapPost("/{clientId}/assets", CreateAssetAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{clientId}/income-assessments", GetIncomeAssessmentsAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapPost("/loan-applications/{applicationId}/income-assessment", CreateIncomeAssessmentAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{clientId}/credit-score", GetCreditScoreAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        return group;
    }

    private static async Task<IResult> GetCreditHistoryAsync(
        long clientId,
        ICreditScoringService creditScoringService,
        CancellationToken cancellationToken)
    {
        var result = await creditScoringService.GetCreditHistoryAsync(clientId, cancellationToken);
        if (result is null)
            return Results.NotFound($"Client with id {clientId} not found");

        return Results.Ok(result);
    }

    private static async Task<IResult> GetClientAssetsAsync(
        long clientId,
        ICreditScoringService creditScoringService,
        CancellationToken cancellationToken)
    {
        var result = await creditScoringService.GetClientAssetsAsync(clientId, cancellationToken);
        if (result is null)
            return Results.NotFound($"Client with id {clientId} not found");

        return Results.Ok(result);
    }

    private static async Task<IResult> CreateAssetAsync(
        long clientId,
        CreateAssetRequest request,
        ICreditScoringService creditScoringService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await creditScoringService.CreateAssetAsync(clientId, request, cancellationToken);
            if (result is null)
                return Results.NotFound($"Client with id {clientId} not found");

            return Results.Created($"/api/clients/{clientId}/assets/{result.Id}", result);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, new List<string>()));
        }
    }

    private static async Task<IResult> GetIncomeAssessmentsAsync(
        long clientId,
        ICreditScoringService creditScoringService,
        CancellationToken cancellationToken)
    {
        var result = await creditScoringService.GetIncomeAssessmentsAsync(clientId, cancellationToken);
        if (result is null)
            return Results.NotFound($"Client with id {clientId} not found");

        return Results.Ok(result);
    }

    private static async Task<IResult> CreateIncomeAssessmentAsync(
        long applicationId,
        CreateIncomeAssessmentRequest request,
        ICreditScoringService creditScoringService,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await creditScoringService.CreateIncomeAssessmentAsync(applicationId, request, cancellationToken);
            if (result is null)
                return Results.NotFound($"Application with id {applicationId} not found");

            return Results.Created($"/api/loan-applications/{applicationId}/income-assessment", result);
        }
        catch (ValidationException ex)
        {
            return Results.BadRequest(new ErrorResponse(ex.Message, new List<string>()));
        }
    }

    private static async Task<IResult> GetCreditScoreAsync(
        long clientId,
        ICreditScoringService creditScoringService,
        CancellationToken cancellationToken)
    {
        var result = await creditScoringService.GetCreditScoreAsync(clientId, cancellationToken);
        if (result is null)
            return Results.NotFound($"Client with id {clientId} not found");

        return Results.Ok(result);
    }
}
