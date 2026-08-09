using TerraLink.Api.Models;
namespace TerraLink.Api.DTOs.Users;
public record OfficerListItem(
    long Id,
    string? EmployeeNo,
    string? Email,
    UserStatus Status,
    DateTime LastLogin
);