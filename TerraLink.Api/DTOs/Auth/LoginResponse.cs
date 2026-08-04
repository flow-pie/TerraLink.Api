namespace TerraLink.Api.DTOs.Auth;

public record LoginUserResponse(
    long Id,
    string RoleName
);
public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    LoginUserResponse User
);