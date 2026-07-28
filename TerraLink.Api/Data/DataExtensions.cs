using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Data
{
    public static class DataExtensions
    {
        public static void DbMigrate(this WebApplication app) {
            //create a scope to get the dbcontext and run migrations
            app.Services.CreateScope().ServiceProvider.GetRequiredService<TerraLinkDbContext>().Database.Migrate();
        }
    }
}
