using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs.Auth
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(120)]
        public string Identifier { get; set; } = string.Empty; // Can be email or username or phone number.

        [Required]
        [MinLength(8)]
        [MaxLength(128)]
        public string Password { get; set; } = string.Empty;
    }
}