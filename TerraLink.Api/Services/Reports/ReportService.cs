using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TerraLink.Api.Data;
using TerraLink.Api.DTOs;
using TerraLink.Api.DTOs.Reports;
using TerraLink.Api.Models;

namespace TerraLink.Api.Services.Reports;

public class ReportService(
    TerraLinkDbContext dbContext
) : IReportService
{
    public async Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(
        CancellationToken cancellationToken)
    {
        var activeLoans = await dbContext.Loans
            .CountAsync(l => l.Status == LoanStatus.ACTIVE, cancellationToken);

        var totalClients = await dbContext.Clients
            .CountAsync(cancellationToken);

        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var disbursedMtd = await dbContext.Disbursments
            .Where(d => d.Status == DisbursementStatus.COMPLETED && d.DisbursementDate >= startOfMonth)
            .SumAsync(d => d.Amount, cancellationToken);

        var outstandingPortfolio = await dbContext.Loans
            .Where(l => l.Status == LoanStatus.ACTIVE || l.Status == LoanStatus.IN_ARREARS)
            .SumAsync(l => l.Balance, cancellationToken);

        return new PortfolioSummaryResponse(
            activeLoans,
            totalClients,
            disbursedMtd,
            outstandingPortfolio
        );
    }

    public async Task<CbkPrudentialReturnResponse> GetCbkPrudentialReturnAsync(
        CancellationToken cancellationToken)
    {
        var totalOutstanding = await dbContext.Loans
            .Where(l => l.Status == LoanStatus.ACTIVE || l.Status == LoanStatus.IN_ARREARS)
            .SumAsync(l => l.Balance, cancellationToken);

        var totalLoans = await dbContext.Loans
            .Where(l => l.Status == LoanStatus.ACTIVE || l.Status == LoanStatus.IN_ARREARS)
            .CountAsync(cancellationToken);

        var nonPerformingLoans = await dbContext.Loans
            .CountAsync(l => l.Status == LoanStatus.IN_ARREARS, cancellationToken);

        var nplRatio = totalLoans > 0 ? Math.Round((double)nonPerformingLoans / totalLoans * 100, 2) : 0;

        var parOver30 = await dbContext.Loans
            .Where(l => l.Status == LoanStatus.IN_ARREARS)
            .CountAsync(cancellationToken);

        var parOver30Ratio = totalLoans > 0 ? Math.Round((double)parOver30 / totalLoans * 100, 2) : 0;

        return new CbkPrudentialReturnResponse(
            nplRatio,
            parOver30Ratio,
            totalOutstanding,
            DateTime.UtcNow
        );
    }

    public async Task<ArrearsAnalysisResponse> GetArrearsAnalysisAsync(
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var arrearsLoans = await dbContext.Loans
            .AsNoTracking()
            .Where(l => l.Status == LoanStatus.IN_ARREARS)
            .ToListAsync(cancellationToken);

        var buckets = new List<ArrearsBucketResponse>();

        foreach (var loan in arrearsLoans)
        {
            var maxOverdueDays = dbContext.RepaymentSchedules
                .Where(rs => rs.LoanId == loan.Id && rs.Status == InstallmentStatus.OVERDUE)
                .Select(rs => today.DayNumber - rs.DueDate.DayNumber)
                .DefaultIfEmpty(0)
                .Max();

            string range = maxOverdueDays switch
            {
                <= 30 => "1-30",
                <= 60 => "31-60",
                _ => "61-90+"
            };

            var existing = buckets.FirstOrDefault(b => b.Range == range);
            if (existing is null)
            {
                buckets.Add(new ArrearsBucketResponse(range, 1, loan.Balance));
            }
            else
            {
                var index = buckets.IndexOf(existing);
                buckets[index] = new ArrearsBucketResponse(range, existing.Count + 1, existing.Amount + loan.Balance);
            }
        }

        return new ArrearsAnalysisResponse(buckets);
    }

    public async Task<List<OfficerPerformanceResponse>> GetOfficerPerformanceAsync(
        CancellationToken cancellationToken)
    {
        var officers = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => u.Role.Name == "Loan Officer")
            .ToListAsync(cancellationToken);

        var results = new List<OfficerPerformanceResponse>();

        foreach (var officer in officers)
        {
            var clientsAcquired = await dbContext.Clients
                .CountAsync(c => c.RegisteredBy == officer.Id && c.VerificationStatus == VerificationStatus.VERIFIED && c.VerifiedAt.HasValue && c.VerifiedAt.Value.Month == DateTime.UtcNow.Month, cancellationToken);

            var officerLoans = await dbContext.Loans
                .Where(l => l.Client.RegisteredBy == officer.Id)
                .ToListAsync(cancellationToken);

            var efficiency = officerLoans.Count > 0
                ? (int)Math.Round(officerLoans.Count(l => l.Status == LoanStatus.COMPLETED) / (double)officerLoans.Count * 100)
                : 0;

            var incentiveTier = efficiency switch
            {
                >= 95 => "Gold",
                >= 85 => "Silver",
                >= 75 => "Bronze",
                _ => "Standard"
            };

            results.Add(new OfficerPerformanceResponse(
                officer.Id,
                officer.EmployeeNo ?? string.Empty,
                efficiency,
                incentiveTier,
                clientsAcquired
            ));
        }

        return results;
    }

    public async Task<byte[]> ExportReportAsync(
        string name,
        string format,
        CancellationToken cancellationToken)
    {
        var content = $"Report: {name}\nGenerated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}\nFormat: {format}";
        return System.Text.Encoding.UTF8.GetBytes(content);
    }
}
