using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Data;

public class TerraLinkDbContext(DbContextOptions<TerraLinkDbContext> options) : DbContext(options)
{
    public DbSet<Models.User> Users => Set<Models.User>();
    public DbSet<Models.Role> Roles => Set<Models.Role>();
    public DbSet<Models.Client> Clients => Set<Models.Client>();
}
