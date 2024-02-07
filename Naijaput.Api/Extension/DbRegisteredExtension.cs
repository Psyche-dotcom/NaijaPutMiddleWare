using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaijaPut.Core.Context;
using NaijaPut.Core.Entities;

namespace Naijaput.Api.Extension
{
    public static class DbRegisteredExtension
    {
        public static void ConfigureDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<NaijaPutContext>()
                    .AddDefaultTokenProviders();

            services.AddDbContext<NaijaPutContext>(dbContextOptions =>
            {
                var connectionString = configuration["ConnectionStrings:localDB"];
                var maxRetryCount = 3;
                var maxRetryDelay = TimeSpan.FromSeconds(10);

                dbContextOptions.UseNpgsql(connectionString, options =>
                {
                    options.EnableRetryOnFailure(maxRetryCount, maxRetryDelay, null);

                });
            });
        }
    }
}
