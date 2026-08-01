using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{
    /// <summary>
    /// Transaction log of repayment attempts, including failed M-Pesa
    /// transactions.
    /// </summary>
    [Table("payments")]
    public class Payment
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
        [Column("schedule_id")]
        public long ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public RepaymentSchedule Schedule { get; set; } = null!;

        [Required]
        [Column("amount", TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column("payment_method")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Column("status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;

        [MaxLength(150)]
        [Column("failure_reason")]
        public string? FailureReason { get; set; }

        [MaxLength(40)]
        [Column("mpesa_reference")]
        public string? MpesaReference { get; set; }

        [Required]
        [Column("payment_date")]
        public DateTime PaymentDate { get; set; }
    }
}