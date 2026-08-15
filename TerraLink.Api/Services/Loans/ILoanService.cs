using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.Loans;

namespace TerraLink.Api.Services.Loans;

public interface ILoanService
{
    Task<PagedResponse<LoanListItemResponse>> GetLoansAsync(
        string? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken
    );

    Task<LoanDetailResponse?> GetLoanDetailAsync(
        long id,
        CancellationToken cancellationToken
    );

    Task<List<ClientLoanResponse>> GetClientLoansAsync(
        long clientId,
        CancellationToken cancellationToken
    );

    Task<bool> IsLoanOwnerAsync(
        long? loanId,
        long userId,
        CancellationToken cancellationToken
    );
}
