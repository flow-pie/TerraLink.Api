using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{

    /// <summary>
    /// Reference entity for borrower cooperatives, used both as a credit-scoring
    /// input and as a lightweight group-management record.
    /// </summary>
    public class Group
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Column("branch_id")]
        public long? BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }

        [Column("meeting_frequency")]
        public MeetingFrequency? MeetingFrequency { get; set; }

        [MaxLength(255)]
        [Column("meeting_location")]
        public string? MeetingLocation { get; set; }

        [Column("primary_officer_id")]
        public long? PrimaryOfficerId { get; set; }

        [ForeignKey(nameof(PrimaryOfficerId))]
        public User? PrimaryOfficer { get; set; }

        [Column("member_count")]
        public int MemberCount { get; set; } = 0;


        [Column("repayment_rate", TypeName = "decimal(5,2)")]
        public decimal? RepaymentRate { get; set; }


        [Column("health_status")]
        public GroupHealthStatus? HealthStatus { get; set; }


        [Column("status")]
        public GroupStatus Status { get; set; } = GroupStatus.ACTIVE;

        // Navigation: members of the group (clients.group_id is the source of truth)
        public ICollection<Client>? Clients { get; set; }
    }
}