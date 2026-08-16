namespace TerraLink.Api.DTOs.RepaymentSchedule;

public record RepaymentScheduleResponse(
    long Id,
    int InstallmentNumber,
    DateOnly DueDate,
    decimal Principal,
    decimal Interest,
    decimal TotalDue,
    TerraLink.Api.Models.InstallmentStatus Status
);
