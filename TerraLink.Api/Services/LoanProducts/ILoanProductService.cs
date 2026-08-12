using TerraLink.Api.DTOs.LoanProducts;

namespace TerraLink.Api.Services.LoanProducts;

public interface ILoanProductService
{
    Task<List<LoanProductResponse>> GetAllAsync(
        bool includeInactive,
        CancellationToken cancellationToken
    );
}