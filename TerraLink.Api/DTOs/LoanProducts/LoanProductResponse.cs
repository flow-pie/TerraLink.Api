using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.LoanProducts;

public record LoanProductResponse(
    long Id,
    string Name,
    decimal MinimumAmount,
    decimal MaximumAmount,
    decimal InterestRate,
    decimal ProcessingFee,
    decimal LateFee,
    int MinimumDuration,
    int MaximumDuration,
    RepaymentFrequency RepaymentFrequency,
    LoanProductStatus Status
);