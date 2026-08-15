namespace TerraLink.Api.DTOs.Loans;

public record ClientLoanResponse(
    long Id,
    string LoanNo,
    decimal ApprovedAmount,
    decimal Balance,
    TerraLink.Api.Models.LoanStatus Status
);
