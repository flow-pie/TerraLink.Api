using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Auth
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashToken(string token);
        DateTime GetRefreshTokenExpiry();
    }
}