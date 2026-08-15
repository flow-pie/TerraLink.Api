using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.LoanApplications;

namespace TerraLink.Api.Services.LoanApplications;
public interface ILoanApplicationService
{
    Task<LoanApplicationResponse?> CreateLoanApplicationAsync(
        long clientId,
        CreateLoanApplicationRequest request,
        CancellationToken cancellationToken
    );

    Task<PagedResponse<LoanApplicationListItemResponse>> GetLoanApplicationsAsync(
        GetLoanApplicationsRequest request,
        CancellationToken cancellationToken
    );

    Task<LoanApplicationDetailResponse?> GetLoanApplicationDetailAsync(
        long id,
        CancellationToken cancellationToken
    );

    Task<AppraiseLoanApplicationResponse?> AppraiseLoanApplicationAsync(
        long id,
        AppraiseLoanApplicationRequest request,
        long officerId,
        CancellationToken cancellationToken
    );

    Task<LoanApplicationStatusResponse?> GetLoanApplicationStatusAsync(
        long id,
        long userId,
        CancellationToken cancellationToken
    );

    Task<bool> IsClientOwnerAsync(
        long clientId,
        long userId,
        CancellationToken cancellationToken
    );
}