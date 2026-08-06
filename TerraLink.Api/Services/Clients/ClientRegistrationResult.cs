using TerraLink.Api.DTOs.Clients;


namespace TerraLink.Api.Services.Clients;

public enum ClientRegistrationStatus
{
    Success,
    DuplicateNationalId,
    DuplicatePhone,
    DuplicateEmail,
    InvalidOfficer
}

public record ClientRegistrationResult(
    ClientRegistrationStatus Status,
    RegisterClientResponse? Response = null
);