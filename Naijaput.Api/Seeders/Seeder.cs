using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaijaPut.Core.Context;
using NaijaPut.Core.Entities;

namespace Naijaput.Api.Seeders
{
    public class Seeder
    {
        public static async Task SeedData(IApplicationBuilder app)
        {
            
            
            var dbContext = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<NaijaPutContext>();

            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
            if (!dbContext.Roles.Any())
            {
                await dbContext.Database.EnsureCreatedAsync();
                var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                // Creating list of roles
                List<string> roles = new() {  "Admin", "User"};

                // Creating roles
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole { Name = role });
                }
            }
            }
    }
}
