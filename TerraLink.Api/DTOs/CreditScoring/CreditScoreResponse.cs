using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.CreditScoring;

public record CreditScoreResponse(
    int CreditScore,
    string Rating
);

public record CreditHistoryResponse(
    string LoanNo,
    int CreditScore,
    string RepaymentRating
);

public record AssetResponse(
    long Id,
    long ClientId,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    AssetType AssetType,
    string? Description,
    int Quantity,
    DateTime RecordedAt
);

public record CreateAssetRequest(
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    AssetType AssetType,
    string? Description,
    int Quantity
);

public record IncomeAssessmentResponse(
    long Id,
    long ClientId,
    long? LoanApplicationId,
    decimal BusinessRevenue,
    decimal OtherIncome,
    decimal HouseholdExpenses,
    decimal DisposableIncome,
    DateTime AssessedAt
);

public record CreateIncomeAssessmentRequest(
    decimal BusinessRevenue,
    decimal OtherIncome,
    decimal HouseholdExpenses
);
