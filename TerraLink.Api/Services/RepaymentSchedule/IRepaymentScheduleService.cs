using TerraLink.Api.DTOs.RepaymentSchedule;

namespace TerraLink.Api.Services.RepaymentSchedule;

public interface IRepaymentScheduleService
{
    Task GenerateAsync(
        long loanId,
        DateTime disbursementDate,
        CancellationToken cancellationToken);
        
    Task<List<RepaymentScheduleResponse>> GetRepaymentScheduleAsync(
        long loanId,
        CancellationToken cancellationToken
    );

    Task<bool> LoanExistsAsync(
        long loanId,
        CancellationToken cancellationToken
    );
}
