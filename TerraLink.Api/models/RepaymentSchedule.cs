using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{


    /// <summary>
    /// Instalment-level repayment plan generated for each active loan.
    /// </summary>
    [Table("repayment_schedule")]
    public class RepaymentSchedule
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("loan_id")]
        public long LoanId { get; set; }

        [ForeignKey(nameof(LoanId))]
        public Loan Loan { get; set; } = null!;

        [Required]
        [Column("installment_number")]
        public int InstallmentNumber { get; set; }

        [Required]
        [Column("due_date", TypeName = "date")]
        public DateOnly DueDate { get; set; }

        [Required]
        [Column("principal", TypeName = "decimal(12,2)")]
        public decimal Principal { get; set; }

        [Required]
        [Column("interest", TypeName = "decimal(12,2)")]
        public decimal Interest { get; set; }

        [Required]
        [Column("total_due", TypeName = "decimal(12,2)")]
        public decimal TotalDue { get; set; }

        [Required]
        [Column("status")]
        public InstallmentStatus Status { get; set; }

        // Navigation
        public ICollection<Payment>? Payments { get; set; }
    }
}