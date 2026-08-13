using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    /// <summary>
    /// Tracks the application's position in the pipeline; set by the system
    /// as the record moves through SUBMITTED -> UNDER_REVIEW -> ... Distinct
    /// </summary>

    public class LoanApplication
    {
        public long Id { get; set; }

        [MaxLength(30)]
        public string? ApplicationNo { get; set; }

        [Required]
        public long ClientId { get; set; }
        public Client Client { get; set; } = null!;

        [Required]
        public long LoanProductId { get; set; }
        public LoanProduct LoanProduct { get; set; } = null!;

        [Required]
        public decimal RequestedAmount { get; set; }

        [Required]
        public int DurationMonths { get; set; }

        [Required]
        public string Purpose { get; set; } = null!;

        [Required]
        public LoanApplicationStatus Status { get; set; } = LoanApplicationStatus.SUBMITTED;

        public long? AppraisedBy { get; set; }

        public User? AppraisedByUser { get; set; }

        public int? CreditScoreSnapshot { get; set; }

        public LoanDecision? Decision { get; set; }

        public string? DecisionNotes { get; set; }

        public DateTime? DecidedAt { get; set; }

        public DateTime SubmittedAt {get; set;} = DateTime.UtcNow;

        // Navigation
        public Loan? Loan { get; set; }
    }
}