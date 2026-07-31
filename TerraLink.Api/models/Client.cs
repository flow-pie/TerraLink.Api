using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TerraLink.Api.Models;

public class Client
{
    public long Id { get; set; }

    [MaxLength(20)]
    public string? ClientNo { get; set; }

    // Linked login account. NULL for an officer-registered client who
    // has not yet installed the app; always populated for SELF channel.
    public long UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [MaxLength(120)]
    public required string FullName { get; set; }

    [MaxLength(8)]
    public required string NationalId { get; set; }

    [MaxLength(10)]
    public required string Phone { get; set; }

    public required DateTime DateOfBirth { get; set; }

    public required Gender gender { get; set; }

    public required string Address { get; set; }

    public long? GroupId { get; set; }

    [Required]
    public RegistrationChannel RegistrationChannel { get; set; }

    // Officer who performed registration; NULL for self-registration.
    public long? RegisteredBy { get; set; }

    [ForeignKey(nameof(RegisteredBy))]
    public User? RegisteredByUser { get; set; }

    [Required]
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.PENDING;

    // Officer who approved verification, where applicable.
    public long? VerifiedBy { get; set; }

    [ForeignKey(nameof(VerifiedBy))]
    public User? VerifiedByUser { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public ClientStatus Status { get; set; } = ClientStatus.ACTIVE;
}