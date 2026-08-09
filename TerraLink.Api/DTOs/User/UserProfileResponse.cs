using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.Users
{

    // // Returned by GET /api/users/me and GET /api/users/{id}. 
    //SHOULD NOT contain all the information such as password hash, MFA secret, etc.
    public record UserProfileResponse
    (
        long Id,
        string? Username,
        string? Email,
        string? EmployeeNo,
        string RoleName,
        UserStatus Status,
        bool MfaEnabled,
        DateTime? LastLogin,
        DateTime CreatedAt
    );
}