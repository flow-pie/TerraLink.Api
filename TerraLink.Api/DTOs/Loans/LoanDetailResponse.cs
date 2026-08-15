using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.Loans;

public record LoanDetailResponse(
    long Id,
    string LoanNo,
    string ClientFullName,
    decimal ApprovedAmount,
    decimal Balance,
    int InstallmentsPaid,
    int InstallmentsTotal,
    DateOnly? NextDueDate,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    TerraLink.Api.Models.LoanStatus Status
);
