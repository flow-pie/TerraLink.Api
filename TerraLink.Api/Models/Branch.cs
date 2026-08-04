using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    public class Branch
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty; // e.g. "North Central Hub"

        [Required]
        [MaxLength(100)]
        [Column("region")]
        public string Region { get; set; } = string.Empty; // e.g. "East Cluster"

        [Column("status")]
        public BranchStatus Status { get; set; } = BranchStatus.ACTIVE;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}