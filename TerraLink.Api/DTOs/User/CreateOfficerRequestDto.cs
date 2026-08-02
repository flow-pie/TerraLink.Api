using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.DTOs
{
     // POST /api/users — admin-only officer account creation.
    // RoleId is intentionally absent: this endpoint always creates a
    // Loan Officer, so employee_no can be validated as required.
    public record CreateOfficerRequestDto
    {
        [MaxLength(20)]
        public required string EmployeeNo { get; set; }

        [EmailAddress]
        [MaxLength(120)]
        public required string Email { get; set; }

        [MaxLength(120)]
        public required string Password { get; set; }
    }
}