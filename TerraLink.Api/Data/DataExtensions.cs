using Microsoft.EntityFrameworkCore;

namespace TerraLink.Api.Data
{
    public static class DataExtensions
    {
        public static void DbMigrate(this WebApplication app) {
            //create a scope to get the dbcontext and run migrations
            var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TerraLinkDbContext>();
            context.Database.Migrate();

            //seed data if needed
            if (!context.Set<Models.Role>().Any())
            {
               
                // Seed initial data
                context.Set<Models.Role>().AddRange(
                    new Models.Role { Name = "Admin", Description = "System Administrator" },
                    new Models.Role { Name = "Client", Description = "Self-service access to loan applications, repayments, and account history." },
                    new Models.Role { Name = "Loan Officer", Description = "Full access to client management, appraisal, and disbursement workflows." }
                );

                context.SaveChanges();
            }
        }
    }
}
