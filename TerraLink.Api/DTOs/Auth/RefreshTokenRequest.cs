using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs.Auth;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}