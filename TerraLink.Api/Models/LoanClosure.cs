using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    /// <summary>
    /// Records completion of a loan upon full repayment.
    /// </summary>
    [Table("loan_closures")]
    public class LoanClosure
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
        [Column("closure_date", TypeName = "date")]
        public DateOnly ClosureDate { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("certificate_number")]
        public string CertificateNumber { get; set; } = null!;

        [Required]
        [Column("closed_by")]
        public long ClosedBy { get; set; }

        [ForeignKey(nameof(ClosedBy))]
        public User ClosedByUser { get; set; } = null!;
    }
}