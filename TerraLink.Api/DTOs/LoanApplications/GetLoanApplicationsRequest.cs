namespace TerraLink.Api.DTOs.LoanApplications;

public class GetLoanApplicationsRequest
{
    public string? Status { get; set; }
    public long? ClientId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
