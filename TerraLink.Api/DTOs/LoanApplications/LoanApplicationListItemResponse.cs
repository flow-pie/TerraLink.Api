using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.LoanApplications;

public record LoanApplicationListItemResponse(
    long Id,
    string ApplicationNo,
    string ClientFullName,
    decimal RequestedAmount,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanApplicationStatus Status,
    DateTime SubmittedAt
);
