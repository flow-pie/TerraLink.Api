using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Data;

//Applied via Db Migrations
public class TerraLinkDbContext(DbContextOptions<TerraLinkDbContext> options) : DbContext(options)
{
    public DbSet<Models.User> Users => Set<Models.User>();
    public DbSet<Models.Role> Roles => Set<Models.Role>();
    public DbSet<Models.Client> Clients => Set<Models.Client>();
    public DbSet<Models.CreditHistory> CreditHistories => Set<Models.CreditHistory>();
    public DbSet<Models.KycDocument> KycDocuments => Set<Models.KycDocument>();
    public DbSet<Models.Branch> Branches => Set<Models.Branch>();
    public DbSet<Models.Group> Groups => Set<Models.Group>();
    public DbSet<Models.LoanProduct> LoanProducts => Set<Models.LoanProduct>();
    public DbSet<Models.LoanApplication> LoanApplications => Set<Models.LoanApplication>();
    public DbSet<Models.Loan> Loans => Set<Models.Loan>();
    public DbSet<Models.IncomeAssessment> IncomeAssessments => Set<Models.IncomeAssessment>();
    public DbSet<Models.RepaymentSchedule> RepaymentSchedules => Set<Models.RepaymentSchedule>();
    public DbSet<Models.Payment> Payments => Set<Models.Payment>();
    public DbSet<Models.Disbursment> Disbursments => Set<Models.Disbursment>();
    public DbSet<Models.LoanClosure> LoanClosures => Set<Models.LoanClosure>();
    public DbSet<Models.Notification> Notifications => Set<Models.Notification>();
    public DbSet<Models.ReportSchedule> ReportSchedules => Set<Models.ReportSchedule>();
    public DbSet<Models.ReportSchedule> Reports => Set<Models.ReportSchedule>();
    public DbSet<Models.AuditLog> AuditLogs => Set<Models.AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Models.Client>(entity =>
        {
            //Client account relationship with User account
            entity.HasOne(client => client.User)
                .WithOne()
                .HasForeignKey<Models.Client>(client => client.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            //officer who registered the client
            entity.HasOne(client => client.RegisteredByUser)
                .WithMany()
                .HasForeignKey(client => client.RegisteredBy)
                .OnDelete(DeleteBehavior.SetNull);

            // Officer who verified the client:
            // Client.VerifiedBy → User.Id
            entity.HasOne(client => client.VerifiedByUser)
                .WithMany()
                .HasForeignKey(client => client.VerifiedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
