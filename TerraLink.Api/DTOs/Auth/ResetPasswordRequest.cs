using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs.Auth;

public class ResetPasswordRequest
{
    [Required]
    public string Token { get; set; }
        = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(128)]
    public string NewPassword { get; set; } = string.Empty;
}