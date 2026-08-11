using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.Models;

public class Client
{
    public long Id { get; set; }

    [MaxLength(20)]
    public string? ClientNo { get; set; }

    // Linked login account. NULL for an officer-registered client who
    // has not yet installed the app; always populated for SELF channel.
    public long? UserId { get; set; }

    public User? User { get; set; }

    [MaxLength(120)]
    public required string FullName { get; set; }

    [MaxLength(8)]
    public required string NationalId { get; set; }

    [MaxLength(10)]
    public required string Phone { get; set; }

    public DateTime DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public required string Address { get; set; }

    public long? GroupId { get; set; }

    public RegistrationChannel RegistrationChannel { get; set; }

    // Officer who performed registration; NULL for self-registration.
    public long? RegisteredBy { get; set; }


    public User? RegisteredByUser { get; set; }

    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.PENDING;

    // Officer who approved verification, where applicable.
    public long? VerifiedBy { get; set; }

    public User? VerifiedByUser { get; set; }

    public DateTime? VerifiedAt { get; set; }

    public ClientStatus Status { get; set; } = ClientStatus.ACTIVE;

    [MaxLength(500)]
    public string? RejectionReason { get; set; }

    public long? RejectedBy { get; set; }
    public DateTime? RejectedAt { get; set; }
}