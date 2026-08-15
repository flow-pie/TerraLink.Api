namespace TerraLink.Api.DTOs.Loans;

public record LoanListItemResponse(
    long Id,
    string LoanNo,
    string ClientFullName,
    decimal ApprovedAmount,
    decimal Balance,
    TerraLink.Api.Models.LoanStatus Status
);
