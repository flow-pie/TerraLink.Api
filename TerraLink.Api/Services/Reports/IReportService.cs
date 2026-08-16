using TerraLink.Api.DTOs.Reports;

namespace TerraLink.Api.Services.Reports;

public interface IReportService
{
    Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(
        CancellationToken cancellationToken
    );

    Task<CbkPrudentialReturnResponse> GetCbkPrudentialReturnAsync(
        CancellationToken cancellationToken
    );

    Task<ArrearsAnalysisResponse> GetArrearsAnalysisAsync(
        CancellationToken cancellationToken
    );

    Task<List<OfficerPerformanceResponse>> GetOfficerPerformanceAsync(
        CancellationToken cancellationToken
    );

    Task<byte[]> ExportReportAsync(
        string name,
        string format,
        CancellationToken cancellationToken
    );
}
