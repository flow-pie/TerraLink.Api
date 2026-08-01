using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{
    /// <summary>
    /// Historical record of a client's credit performance across loans,
    /// used in future scoring.
    /// </summary>
    [Table("credit_history")]
    public class CreditHistory
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Required]
        [Column("loan_id")]
        public long LoanId { get; set; }

        [ForeignKey(nameof(LoanId))]
        public Loan Loan { get; set; } = null!;

        [Required]
        [Column("credit_score")]
        public int CreditScore { get; set; }

        /// <summary>Qualitative rating (e.g. 'A-Prime').</summary>
        [Required]
        [MaxLength(20)]
        [Column("repayment_rating")]
        public string RepaymentRating { get; set; } = null!;
    }
}