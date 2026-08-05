using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs.Auth;

public class MfaVerifyRequest
{
    [Required]
    public string MfaToken { get; set; } = string.Empty;

    [Required]
    [RegularExpression(
        @"^\d{6}$",
        ErrorMessage = "MFA code must contain exactly 6 digits."
    )]
    public string Code { get; set; } = string.Empty;
}