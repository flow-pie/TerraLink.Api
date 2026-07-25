using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.Models;
public class Client
{
    public long Id { get; set; }

    [MaxLength(20)]
    [Index(nameof(ClientNo), IsUnique = true)]
    public string? ClientNo { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    [MaxLength(120)]
    public required string ClientName { get; set; }

    [MaxLength(8)]
    public required string NationalId { get; set; }

    [MaxLength(10)]
    public required string Phone { get; set; }

    public required DateTime DateOfBirth { get; set; }
    public required Gender gender { get; set; }

    public long? RegisteredById { get; set; }
    public User? RegisteredBy { get; set; } 

    public long? VerifiedById { get; set; }
    public User? VerifiedBy { get; set; }
    public DateTime VerifiedAt { get; set; }

    public enum Gender
    {
        Male,
        Female,
        Other
    }

}