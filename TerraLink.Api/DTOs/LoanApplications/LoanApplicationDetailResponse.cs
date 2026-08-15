using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.LoanApplications;

public record LoanApplicationDetailResponse(
    ApplicationDetail Application,
    ClientDetail Client,
    CreditScoreDetail CreditScore,
    List<CreditHistoryDetail> CreditHistory,
    IncomeAssessmentDetail IncomeAssessment,
    List<AssetDetail> Assets
);

public record ApplicationDetail(
    long Id,
    string ApplicationNo,
    decimal RequestedAmount,
    int DurationMonths,
    string Purpose,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanApplicationStatus Status
);

public record ClientDetail(
    long Id,
    string FullName,
    string? ClientNo
);

public record CreditScoreDetail(
    int Score,
    string Rating
);

public record CreditHistoryDetail(
    string LoanNo,
    int CreditScore,
    string RepaymentRating,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanStatus Status
);

public record IncomeAssessmentDetail(
    decimal BusinessRevenue,
    decimal OtherIncome,
    decimal HouseholdExpenses,
    decimal DisposableIncome
);

public record AssetDetail(
    AssetType AssetType,
    string? Description,
    int Quantity
);
