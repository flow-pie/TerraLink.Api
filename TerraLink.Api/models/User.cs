using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Models;

public class User
{
    public long Id { get; set; }

    [MaxLength(20)]
    public string? Username { get; set; } //can be phone number or employee id or email

    [MaxLength(120)]
    public string? Email { get; set; }

    [MaxLength(255)]
    public required string PasswordHash { get; set; }

    public long RoleId { get; set; } 
    public Role Role { get; set; } = null!;
    public required UserStatus Status { get; set; } = UserStatus.Active;
    public bool MfaEnabled { get; set; }

    [MaxLength(255)]
    public string? MfaSecret { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public enum UserStatus
    {
        Active,
        Inactive,
        Suspended,
        Locked,
        PendingVerification,

    }
    
}