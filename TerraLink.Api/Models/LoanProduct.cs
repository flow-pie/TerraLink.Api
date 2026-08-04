using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    /// <summary>
    /// Master catalogue of loan products offered, including interest, fees,
    /// and eligibility bounds.
    /// </summary>
    [Table("loan_products")]
    public class LoanProduct
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = null!;

        [Required]
        [Column("minimum_amount", TypeName = "decimal(12,2)")]
        public decimal MinimumAmount { get; set; }

        [Required]
        [Column("maximum_amount", TypeName = "decimal(12,2)")]
        public decimal MaximumAmount { get; set; }

        [Required]
        [Column("interest_rate", TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Column("processing_fee", TypeName = "decimal(12,2)")]
        public decimal ProcessingFee { get; set; } = 0;

        [Column("late_fee", TypeName = "decimal(12,2)")]
        public decimal LateFee { get; set; } = 0;

        [Required]
        [Column("minimum_duration")]
        public int MinimumDuration { get; set; }

        [Required]
        [Column("maximum_duration")]
        public int MaximumDuration { get; set; }

        [Required]
        [Column("repayment_frequency")]
        public RepaymentFrequency RepaymentFrequency { get; set; }

        [Column("status")]
        public LoanProductStatus Status { get; set; } = LoanProductStatus.ACTIVE;

        // Navigation
        public ICollection<LoanApplication>? LoanApplications { get; set; }
    }
}