namespace TerraLink.Api.DTOs.Clients;

public record RegisterClientResponse(
    long ClientId,
    long? UserId,
    string ClientNo,
    string RegistrationChannel,
    string VerificationStatus,
    string Message
);