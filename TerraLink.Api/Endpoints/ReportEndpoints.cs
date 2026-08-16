using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.Reports;
using TerraLink.Api.Services.Reports;

namespace TerraLink.Api.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports")
            .WithTags("Reports");

        group.MapGet("/portfolio-summary", GetPortfolioSummaryAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/cbk-prudential-return", GetCbkPrudentialReturnAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/arrears-analysis", GetArrearsAnalysisAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/officer-performance", GetOfficerPerformanceAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/{name}/export", ExportReportAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        group.MapGet("/report-schedules", GetReportSchedulesAsync)
            .RequireAuthorization(policy =>
                policy.RequireRole("Loan Officer"));

        return group;
    }

    private static async Task<IResult> GetPortfolioSummaryAsync(
        IReportService reportService,
        CancellationToken cancellationToken)
    {
        var result = await reportService.GetPortfolioSummaryAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetCbkPrudentialReturnAsync(
        IReportService reportService,
        CancellationToken cancellationToken)
    {
        var result = await reportService.GetCbkPrudentialReturnAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetArrearsAnalysisAsync(
        IReportService reportService,
        CancellationToken cancellationToken)
    {
        var result = await reportService.GetArrearsAnalysisAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetOfficerPerformanceAsync(
        IReportService reportService,
        CancellationToken cancellationToken)
    {
        var result = await reportService.GetOfficerPerformanceAsync(cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> ExportReportAsync(
        string name,
        string format,
        IReportService reportService,
        CancellationToken cancellationToken)
    {
        var validNames = new[] { "portfolio-summary", "cbk-prudential-return", "arrears-analysis", "officer-performance" };
        if (!validNames.Contains(name))
        {
            return Results.BadRequest(new ErrorResponse("Unknown report name.", new List<string> { nameof(name) }));
        }

        var validFormats = new[] { "pdf", "csv" };
        if (!validFormats.Contains(format.ToLower()))
        {
            return Results.BadRequest(new ErrorResponse("Unsupported format.", new List<string> { nameof(format) }));
        }

        var bytes = await reportService.ExportReportAsync(name, format, cancellationToken);
        var contentType = format.ToLower() == "pdf" ? "application/pdf" : "text/csv";
        return Results.File(bytes, contentType);
    }

    private static async Task<IResult> GetReportSchedulesAsync(
        TerraLink.Api.Data.TerraLinkDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var schedules = await dbContext.ReportSchedules
            .AsNoTracking()
            .OrderByDescending(rs => rs.NextRun)
            .Select(rs => new TerraLink.Api.DTOs.Reports.ReportScheduleResponse(
                rs.Id,
                rs.ReportName,
                rs.Frequency,
                rs.NextRun,
                rs.Enabled
            ))
            .ToListAsync(cancellationToken);

        return Results.Ok(schedules);
    }
}
