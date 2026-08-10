using System.Text.Json.Serialization;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.Clients
{
    //for GET /api/users list view.
    public record ClientProfileResponse
    (
        //from users entity
        long Id,
        string? Username,
        string? Email,
        string? EmployeeNo,
        string RoleName,
        DateTime? LastLogin,

        //from client entity
        string? NationalId,
        string? Phone,
        string? FullName,
        DateTime? DateOfBirth,
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        Gender? Gender,
        DateTime? VerifiedAt,
        string? Address,
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        ClientStatus UserStatus,
        [property: JsonConverter(typeof(JsonStringEnumConverter))]
        VerificationStatus? VerificationStatus
    );
}