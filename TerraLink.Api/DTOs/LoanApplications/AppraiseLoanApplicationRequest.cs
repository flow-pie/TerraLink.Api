using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.LoanApplications;

public record AppraiseLoanApplicationRequest(
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanDecision Decision,
    string? DecisionNotes,
    int? CreditScoreSnapshot
);

public record AppraiseLoanApplicationResponse(
    long Id,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanApplicationStatus Status,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanDecision? Decision,
    DateTime? DecidedAt,
    long? CreatedLoanId
);

public record LoanApplicationStatusResponse(
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    LoanApplicationStatus Status,
    List<StatusTimelineItem> Timeline,
    AssignedOfficer? AssignedOfficer
);

public record StatusTimelineItem(
    string Stage,
    DateTime? CompletedAt
);

public record AssignedOfficer(
    string FullName
);
