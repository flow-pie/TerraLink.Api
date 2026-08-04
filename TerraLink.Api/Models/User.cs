using System.ComponentModel.DataAnnotations;

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
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public bool MfaEnabled { get; set; } = false;

    [MaxLength(255)]
    public string? MfaSecret { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    //navigation when role==client
    public Client? Client { get; set; }

    //NOTE Roles are seeded with fixed IDs, so we can use the RoleId to determine if a user is a loan officer or not.
    private const long LoanOfficerRoleId = 3;

    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        var isLoanOfficer = RoleId == LoanOfficerRoleId;

        if (isLoanOfficer &&
            string.IsNullOrWhiteSpace(EmployeeNo))
        {
            yield return new ValidationResult(
                "Employee number is required for Loan Officers.",
                new[] { nameof(EmployeeNo) });
        }

        if (!isLoanOfficer &&
            !string.IsNullOrWhiteSpace(EmployeeNo))
        {
            yield return new ValidationResult(
                "Employee number must be empty for Clients.",
                new[] { nameof(EmployeeNo) });
        }
    }
}