namespace TerraLink.Api.Models;

public class PasswordResetToken
{
    public long Id { get; set; }

    public long UserId { get; set; }

    // SHA-256 hash of the raw token.
    // No raw token in the database.
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public bool IsUsable =>
        UsedAt is null &&
        ExpiresAt > DateTime.UtcNow;
}