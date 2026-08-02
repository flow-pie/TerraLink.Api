using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    /// <summary>
    /// Tracks the application's position in the pipeline; set by the system
    /// as the record moves through SUBMITTED -> UNDER_REVIEW -> ... Distinct
    /// </summary>

    [Table("loan_applications")]
    public class LoanApplication
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("application_no")]
        public string ApplicationNo { get; set; } = null!;

        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Required]
        [Column("loan_product_id")]
        public long LoanProductId { get; set; }

        [ForeignKey(nameof(LoanProductId))]
        public LoanProduct LoanProduct { get; set; } = null!;

        [Required]
        [Column("requested_amount", TypeName = "decimal(12,2)")]
        public decimal RequestedAmount { get; set; }

        [Required]
        [Column("duration_months")]
        public int DurationMonths { get; set; }

        [Required]
        [Column("purpose")]
        public string Purpose { get; set; } = null!;

        [Required]
        [Column("status")]
        public LoanApplicationStatus Status { get; set; } = LoanApplicationStatus.SUBMITTED;

        [Column("appraised_by")]
        public long? AppraisedBy { get; set; }

        [ForeignKey(nameof(AppraisedBy))]
        public User? AppraisedByUser { get; set; }

        [Column("credit_score_snapshot")]
        public int? CreditScoreSnapshot { get; set; }

        [Column("decision")]
        public LoanDecision? Decision { get; set; }

        [Column("decision_notes")]
        public string? DecisionNotes { get; set; }

        [Column("decided_at")]
        public DateTime? DecidedAt { get; set; }

        // Navigation
        public Loan? Loan { get; set; }
    }
}