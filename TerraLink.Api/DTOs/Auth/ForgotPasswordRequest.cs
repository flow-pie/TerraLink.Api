using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required]
    [MaxLength(120)]
    public string Identifier { get; set; }
        = string.Empty;
}