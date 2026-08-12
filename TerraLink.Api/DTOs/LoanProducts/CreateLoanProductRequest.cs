using System.ComponentModel.DataAnnotations;
using TerraLink.Api.Models;

namespace TerraLink.Api.DTOs.LoanProducts;

public class CreateLoanProductRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal MinimumAmount { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal MaximumAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal InterestRate { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ProcessingFee { get; set; }

    [Range(0, double.MaxValue)]
    public decimal LateFee { get; set; }

    [Range(1, int.MaxValue)]
    public int MinimumDuration { get; set; }

    [Range(1, int.MaxValue)]
    public int MaximumDuration { get; set; }

    public RepaymentFrequency RepaymentFrequency { get; set; }
}