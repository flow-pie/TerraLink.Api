namespace TerraLink.Api.DTOs.Auth;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);