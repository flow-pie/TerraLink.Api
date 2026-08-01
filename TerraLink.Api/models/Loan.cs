using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Models
{
    /// <summary>
    /// Represents an approved, active loan created once a loan_application
    /// is approved.
    /// </summary>
    [Table("loans")]
    public class Loan
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("loan_no")]
        public string LoanNo { get; set; } = null!;

        [Required]
        [Column("application_id")]
        public long ApplicationId { get; set; }

        [ForeignKey(nameof(ApplicationId))]
        public LoanApplication Application { get; set; } = null!;

        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Required]
        [Column("approved_amount", TypeName = "decimal(12,2)")]
        public decimal ApprovedAmount { get; set; }

        [Required]
        [Column("balance", TypeName = "decimal(12,2)")]
        public decimal Balance { get; set; }

        [Required]
        [Column("status")]
        public LoanStatus Status { get; set; }
    }
}