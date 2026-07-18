namespace TerraLink.Api.DTOs;

//A DTO (Data Transfer Object) is a simple object that is used to transfer data between different layers of an application. 
//It typically contains only the data that needs to be transferred, without any business logic or behavior.
public record UserDto(
    long Id,
    string? EmployeeId,
    string FullName,
    string? Email,
    string Phone,
    string Password,
    string Role,
    string Status,
    bool MfaEnabled,
    string MfaSecret,
    DateTime? LastLogin,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);