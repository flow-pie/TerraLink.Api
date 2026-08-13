using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.LoanApplications;

public record LoanApplicationResponse(
    long Id,
    string ApplicationNo,
    long ClientId,
    long LoanProductId,
    decimal RequestedAmount,
    int DurationMonths,
    string Purpose,

    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanApplicationStatus Status,

    DateTime SubmittedAt
);