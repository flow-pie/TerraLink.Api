using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TerraLink.Api.Common;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs.RepaymentSchedule;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.RepaymentSchedule;

public class RepaymentScheduleService(
    TerraLinkDbContext dbContext
) : IRepaymentScheduleService
{
    public async Task<List<RepaymentScheduleResponse>> GetRepaymentScheduleAsync(
        long loanId,
        CancellationToken cancellationToken)
    {
        return await dbContext.RepaymentSchedules
            .AsNoTracking()
            .Where(rs => rs.LoanId == loanId)
            .OrderBy(rs => rs.InstallmentNumber)
            .Select(rs => new RepaymentScheduleResponse(
                rs.Id,
                rs.InstallmentNumber,
                rs.DueDate,
                rs.Principal,
                rs.Interest,
                rs.TotalDue,
                rs.Status
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> LoanExistsAsync(
        long loanId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Loans.AnyAsync(l => l.Id == loanId, cancellationToken);
    }

    //repayment schedule helpers
    public async Task GenerateAsync(
    long loanId,
    DateTime disbursementDate,
    CancellationToken cancellationToken)
    {

        var loan = await dbContext.Loans
            .Include(l => l.Application)
            .ThenInclude(a => a.LoanProduct)
            .FirstOrDefaultAsync(
                l => l.Id == loanId,
                cancellationToken);

        if (loan is null)
            throw new NotFoundException("Loan not found.");

        var exists = await dbContext.RepaymentSchedules
            .AnyAsync(
                r => r.LoanId == loanId,
                cancellationToken);

        if (exists)
            throw new ConflictException(
                "A repayment schedule already exists for this loan.");

        var numberOfInstallments = loan.Application.DurationMonths;

        if (numberOfInstallments <= 0)
            throw new ValidationException(
                "Loan term must be greater than zero.");

        var monthlyPrincipal =
            loan.ApprovedAmount / numberOfInstallments;

        var monthlyInterest =
            loan.ApprovedAmount * loan.Application.LoanProduct.InterestRate / 100m;

        var schedules = new List<Models.RepaymentSchedule>();

        var remainingBalance = loan.ApprovedAmount;

        for (var i = 1; i <= numberOfInstallments; i++)
        {
            var principal = i == numberOfInstallments
                ? remainingBalance
                : monthlyPrincipal;

            var totalDue = principal + monthlyInterest;

            remainingBalance -= principal;

            schedules.Add(new Models.RepaymentSchedule
            {
                LoanId = loan.Id,
                InstallmentNumber = i,
                DueDate = DateOnly.FromDateTime(disbursementDate.AddMonths(i)),
                Principal = principal,
                Interest = monthlyInterest,
                TotalDue = totalDue,
                Status = InstallmentStatus.PENDING
            });
        }

        dbContext.RepaymentSchedules.AddRange(schedules);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
