using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{
    /// <summary>
    /// Immutable log of all Loan Officer and Client actions across the
    /// system, for compliance and traceability.
    /// </summary>
    [Table("audit_log")]
    public class AuditLog
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
        [MaxLength(100)]
        [Column("action")]
        public string Action { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("entity_type")]
        public string EntityType { get; set; } = null!;

        [Required]
        [Column("entity_id")]
        public long EntityId { get; set; }

        /// <summary>
        /// Structured detail, e.g. changed field values. Stored as raw JSON
        /// text; deserialize with System.Text.Json where needed rather than
        /// mapping to a strong type, since shape varies by action.
        /// </summary>
        [Column("details", TypeName = "json")]
        public string? Details { get; set; }

        /// <summary>Originating IP address.</summary>
        [MaxLength(45)]
        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}