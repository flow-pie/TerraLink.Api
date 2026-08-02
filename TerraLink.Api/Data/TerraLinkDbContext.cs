using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Data;

public class TerraLinkDbContext(DbContextOptions<TerraLinkDbContext> options) : DbContext(options)
{
    public DbSet<Models.User> Users => Set<Models.User>();
    public DbSet<Models.Role> Roles => Set<Models.Role>();
    public DbSet<Models.Client> Clients => Set<Models.Client>();

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
