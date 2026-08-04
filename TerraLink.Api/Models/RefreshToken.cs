namespace TerraLink.Api.Models;

public class RefreshToken
{
    public long Id { get; set; }

    public long UserId { get; set; }

    // Store a SHA-256 hash.
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? RevokedAt { get; set; }

    public string? ReplacedByTokenHash { get; set; }

    public User User { get; set; } = null!;

    public bool IsActive =>
        RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}