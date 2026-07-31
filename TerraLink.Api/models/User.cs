using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Models;

public class User : IValidatableObject
{
    public long Id { get; set; }

    [MaxLength(20)]
    public string? Username { get; set; } //can be phone number or employee id or email

    [MaxLength(120)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? EmployeeNo { get; set; }

    [MaxLength(255)]
    public required string PasswordHash { get; set; }

    public long RoleId { get; set; }
    public Role Role { get; set; } = null!;

    // public long? BranchId {get; set;}
    // [ForeignKey(nameof(BranchId))]
    // public Branch Branch {get; set;}
    public required UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public bool MfaEnabled { get; set; } = false;

    [MaxLength(255)]
    public string? MfaSecret { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    //navigation when role==client
    public Client? Client { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var isLoanOfficer = Role?.Name == "Loan Officer";

        if (isLoanOfficer && string.IsNullOrWhiteSpace(EmployeeNo))
        {
            yield return new ValidationResult(
                "employee_no is required for users with the Loan Officer role.",
                new[] { nameof(EmployeeNo) }
            );
        }

        if (!isLoanOfficer && !string.IsNullOrWhiteSpace(EmployeeNo))
        {
            yield return new ValidationResult(
                "employee_no must be NULL for users with the Client role.",
                new[] { nameof(EmployeeNo) });
        }
    }
}