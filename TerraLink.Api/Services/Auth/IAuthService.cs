using TerraLink.Api.DTOs.Auth;

namespace TerraLink.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
        Task<RefreshTokenResponse?> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken);
        Task<bool> LogoutAsync(long userId, string refreshToken, CancellationToken cancellationToken);
        Task RequestPasswordResetAsync(ForgotPasswordRequest request, CancellationToken cancellationToken);
        Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken );
    }
}