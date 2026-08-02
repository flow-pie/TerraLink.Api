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

   protected override void OnModelCreating(
    ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    
    // CLIENT RELATIONSHIPS
    // =========================

    modelBuilder.Entity<Models.Client>(entity =>
    {
        // Client.UserId → User.Id
        // Optional one-to-one relationship.
        entity.HasOne(client => client.User)
            .WithOne(user => user.Client)
            .HasForeignKey<Models.Client>(
                client => client.UserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Client.RegisteredBy → User.Id
        // One officer can register many clients.
        entity.HasOne(
                client => client.RegisteredByUser)
            .WithMany()
            .HasForeignKey(
                client => client.RegisteredBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Client.VerifiedBy → User.Id
        // One officer can verify many clients.
        entity.HasOne(
                client => client.VerifiedByUser)
            .WithMany()
            .HasForeignKey(
                client => client.VerifiedBy)
            .OnDelete(DeleteBehavior.Restrict);

        // Store Client enums as strings.
        entity.Property(client => client.Gender)
            .HasConversion<string>()
            .HasMaxLength(10);

        entity.Property(
                client => client.RegistrationChannel)
            .HasConversion<string>()
            .HasMaxLength(20);

        entity.Property(
                client => client.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        entity.Property(client => client.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    });

   
    // USER CONFIGURATION
    // =========================

    modelBuilder.Entity<Models.User>(entity =>
    {
        entity.Property(user => user.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // User.RoleId → Role.Id
        // One role can belong to many users.
        entity.HasOne(user => user.Role)
            .WithMany(role => role.Users)
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    });

    // BRANCH CONFIGURATION    
    // =========================

    modelBuilder.Entity<Models.Branch>(entity =>
    {
        entity.Property(branch => branch.Status)
            .HasConversion<string>()
            .HasMaxLength(20);
    });
}
}
