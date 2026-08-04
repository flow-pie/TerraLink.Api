using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TerraLink.Api.Models;

namespace TerraLink.Api.Models
{
    /// <summary>
    /// Point-in-time snapshot of a client's monthly cash flow, captured
    /// during credit appraisal.
    /// </summary>
    [Table("income_assessments")]
    public class IncomeAssessment
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("client_id")]
        public long ClientId { get; set; }

        [ForeignKey(nameof(ClientId))]
        public Client Client { get; set; } = null!;

        [Column("loan_application_id")]
        public long? LoanApplicationId { get; set; }

        [ForeignKey(nameof(LoanApplicationId))]
        public LoanApplication? LoanApplication { get; set; }

        [Column("business_revenue", TypeName = "decimal(12,2)")]
        public decimal BusinessRevenue { get; set; } = 0;

        [Column("other_income", TypeName = "decimal(12,2)")]
        public decimal OtherIncome { get; set; } = 0;

        [Column("household_expenses", TypeName = "decimal(12,2)")]
        public decimal HouseholdExpenses { get; set; } = 0;

        /// <summary>
        /// (BusinessRevenue + OtherIncome) - HouseholdExpenses, written by the
        /// application layer at save time — not computed on read, so this
        /// stays a stable snapshot even if the underlying inputs change later.
        /// </summary>
        [Required]
        [Column("disposable_income", TypeName = "decimal(12,2)")]
        public decimal DisposableIncome { get; set; }

        [Column("assessed_at")]
        public DateTime AssessedAt { get; set; } = DateTime.UtcNow;
    }
}