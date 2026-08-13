namespace TerraLink.Api.DTOs.LoanApplications;

public class CreateLoanApplicationRequest
{
    public long LoanProductId { get; set; }

    public decimal RequestedAmount { get; set; }

    public int DurationMonths { get; set; }

    public string Purpose { get; set; } = string.Empty;
}