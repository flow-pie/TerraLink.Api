using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{

    /// <summary>
    /// In-app notification queue for both Loan Officer and Client users.
    /// </summary>
    [Table("notifications")]
    public class Notification
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        [Column("type")]
        public NotificationType Type { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        [Column("body")]
        public string Body { get; set; } = null!;

        [MaxLength(50)]
        [Column("related_entity_type")]
        public string? RelatedEntityType { get; set; }

        [Column("related_entity_id")]
        public long? RelatedEntityId { get; set; }

        [Column("is_read")]
        public bool IsRead { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}