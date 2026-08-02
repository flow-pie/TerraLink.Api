using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    /// <summary>
    /// Records the release of approved loan funds to the client via M-Pesa.
    /// </summary>
    [Table("disbursements")]
    public class Disbursment
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
        [Column("amount", TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column("disbursement_date")]
        public DateTime DisbursementDate { get; set; }

        [MaxLength(50)]
        [Column("mpesa_reference")]
        public string? MpesaReference { get; set; }

        [Required]
        [Column("status")]
        public DisbursementStatus Status { get; set; }

        [Column("disbursed_by")]
        public long? DisbursedBy { get; set; }

        [ForeignKey(nameof(DisbursedBy))]
        public User? DisbursedByUser { get; set; }
    }
}