using TerraLink.Api.DTOs.LoanApplications;

namespace TerraLink.Api.Services.LoanApplications;
public interface ILoanApplicationService
{
    Task<LoanApplicationResponse?> CreateLoanApplicationAsync(
        long clientId,
        CreateLoanApplicationRequest request,
        CancellationToken cancellationToken
    );
}