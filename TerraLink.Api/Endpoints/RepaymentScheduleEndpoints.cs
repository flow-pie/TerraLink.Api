using System.Security.Claims;
using TerraLink.Api.Common;
using TerraLink.Api.DTOs.Loans;
using TerraLink.Api.Services.Loans;
using TerraLink.Api.Services.RepaymentSchedule;

namespace TerraLink.Api.Endpoints;

public static class RepaymentScheduleEndpoints
{
    public static IEndpointRouteBuilder MapRepaymentScheduleEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/loans")
            .WithTags("Repayment Schedule");

        group.MapGet("/{loanId}/repayment-schedule", GetRepaymentScheduleAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer", "Client"));

        group.MapGet("/{loanId}/repayment-schedule/export", ExportRepaymentScheduleAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer", "Client"));

        return group;
    }

    private static async Task<IResult> GetRepaymentScheduleAsync(
        long loanId,
        ClaimsPrincipal principal,
        IRepaymentScheduleService repaymentScheduleService,
        ILoanService loanService,
        CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();
        var isLoanOfficer = principal.IsInRole("Loan Officer");

        if (!isLoanOfficer)
        {
            var loanExists = await repaymentScheduleService.LoanExistsAsync(loanId, cancellationToken);
            if (!loanExists)
                return Results.NotFound($"Loan with id {loanId} not found");

            var isOwner = await loanService.IsLoanOwnerAsync(loanId, loggedInUserId, cancellationToken);
            if (!isOwner)
                return Results.Forbid();
        }

        var result = await repaymentScheduleService.GetRepaymentScheduleAsync(loanId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ExportRepaymentScheduleAsync(
        long loanId,
        ClaimsPrincipal principal,
        IRepaymentScheduleService repaymentScheduleService,
        ILoanService loanService,
        CancellationToken cancellationToken)
    {
        var loggedInUserId = principal.GetUserId();
        var isLoanOfficer = principal.IsInRole("Loan Officer");

        if (!isLoanOfficer)
        {
            var loanExists = await repaymentScheduleService.LoanExistsAsync(loanId, cancellationToken);
            if (!loanExists)
                return Results.NotFound($"Loan with id {loanId} not found");

            var isOwner = await loanService.IsLoanOwnerAsync(loanId, loggedInUserId, cancellationToken);
            if (!isOwner)
                return Results.Forbid();
        }

        var schedule = await repaymentScheduleService.GetRepaymentScheduleAsync(loanId, cancellationToken);
        var pdfBytes = PdfGenerator.GenerateRepaymentSchedulePdf(schedule);
        return Results.File(pdfBytes, "application/pdf", "repayment-schedule.pdf");
    }
}
