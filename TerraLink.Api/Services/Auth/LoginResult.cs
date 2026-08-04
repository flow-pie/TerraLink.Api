using TerraLink.Api.DTOs.Auth;

namespace TerraLink.Api.Services.Auth
{
    public enum LoginStatus
    {
        Success,
        InvalidCredentials,
        AccountInactive,
        MfaRequired,
        AccountLocked,
        AccountDisabled,      
        MfaFailed
    }
    public record LoginResult
    (
        LoginStatus Status, 
        LoginResponse? Response = null
    );
}