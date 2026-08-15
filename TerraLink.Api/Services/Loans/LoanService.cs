using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.Loans;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Loans;

public class LoanService(
    TerraLinkDbContext dbContext
) : ILoanService
{
    public async Task<PagedResponse<LoanListItemResponse>> GetLoansAsync(
        string? status,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Loans
            .AsNoTracking()
            .Include(l => l.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<LoanStatus>(status, true, out var loanStatus))
        {
            query = query.Where(l => l.Status == loanStatus);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(l =>
                l.LoanNo.ToLower().Contains(searchLower) ||
                l.Client.FullName.ToLower().Contains(searchLower));
        }

        query = query.OrderByDescending(l => l.Id);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LoanListItemResponse(
                l.Id,
                l.LoanNo,
                l.Client.FullName,
                l.ApprovedAmount,
                l.Balance,
                l.Status
            ))
            .ToListAsync(cancellationToken);

        return new PagedResponse<LoanListItemResponse>(
            items,
            page,
            pageSize,
            totalCount
        );
    }

    public async Task<LoanDetailResponse?> GetLoanDetailAsync(
        long id,
        CancellationToken cancellationToken)
    {
        var loan = await dbContext.Loans
            .AsNoTracking()
            .Include(l => l.Client)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        if (loan is null)
            return null;

        var installments = await dbContext.RepaymentSchedules
            .AsNoTracking()
            .Where(rs => rs.LoanId == id)
            .ToListAsync(cancellationToken);

        var installmentsPaid = installments.Count(i => i.Status == InstallmentStatus.PAID);
        var installmentsTotal = installments.Count;
        var nextDueDate = installments
            .Where(i => i.Status == InstallmentStatus.PENDING)
            .OrderBy(i => i.DueDate)
            .Select(i => (DateOnly?)i.DueDate)
            .FirstOrDefault();

        return new LoanDetailResponse(
            loan.Id,
            loan.LoanNo,
            loan.Client.FullName,
            loan.ApprovedAmount,
            loan.Balance,
            installmentsPaid,
            installmentsTotal,
            nextDueDate,
            loan.Status
        );
    }

    public async Task<List<ClientLoanResponse>> GetClientLoansAsync(
        long clientId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Loans
            .AsNoTracking()
            .Where(l => l.ClientId == clientId)
            .OrderByDescending(l => l.Id)
            .Select(l => new ClientLoanResponse(
                l.Id,
                l.LoanNo,
                l.ApprovedAmount,
                l.Balance,
                l.Status
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsLoanOwnerAsync(
        long? loanId,
        long userId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Loans
            .AnyAsync(l => l.Client.UserId == userId, cancellationToken);
    }
}
