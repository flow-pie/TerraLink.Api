namespace TerraLink.Api.DTOs.Auth;

public record MfaChallengeResponse(
    bool MfaRequired,
    string MfaToken
);