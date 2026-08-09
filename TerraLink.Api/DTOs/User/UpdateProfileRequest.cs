using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs.Users
{
      // PATCH /api/users/me — a user may only touch their own contact info
    // and MFA toggle. Null fields are left unchanged cause this is a PATCH not PUT.
    public class UpdateProfileRequest
    {
        [MaxLength(20)]
        public string? Username { get; set; }
        
        [EmailAddress]
        [MaxLength(120)]
        public string? Email { get; set; }
        public bool? MfaEnabled { get; set; }
    }
}