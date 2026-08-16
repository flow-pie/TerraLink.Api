namespace TerraLink.Api.DTOs.Reports;

public record PortfolioSummaryResponse(
    int ActiveLoans,
    int TotalClients,
    decimal DisbursedMtd,
    decimal OutstandingPortfolio
);

public record CbkPrudentialReturnResponse(
    double NplRatio,
    double ParOver30,
    decimal TotalOutstanding,
    DateTime GeneratedAt
);

public record ArrearsBucketResponse(
    string Range,
    int Count,
    decimal Amount
);

public record ArrearsAnalysisResponse(
    List<ArrearsBucketResponse> Buckets
);

public record OfficerPerformanceResponse(
    long OfficerId,
    string EmployeeNo,
    int Efficiency,
    string IncentiveTier,
    int ClientsAcquiredMtd
);

public record ReportScheduleResponse(
    long Id,
    string ReportName,
    TerraLink.Api.Models.ReportFrequency Frequency,
    DateTime NextRun,
    bool Enabled
);
