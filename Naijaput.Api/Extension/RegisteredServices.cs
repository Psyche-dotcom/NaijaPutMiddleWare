using Inventrify.Core.Repository.Implementation;
using Naijaput.Api.ProjectProfileMapping;
using NaijaPut.Core.Repository.Implementation;
using NaijaPut.Core.Repository.Interface;
using NaijaPut.Infrastructure.Service.Implementation;
using NaijaPut.Infrastructure.Service.Interface;

namespace Naijaput.Api.Extension
{
    public static class RegisteredServices
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountRepo, AccountRepo>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailServices, EmailService>();
            services.AddScoped<IGenerateJwt, GenerateJwt>();
            services.AddScoped(typeof(INaijaPutRepository<>), typeof(NaijaPutRepository<>));
            services.AddAutoMapper(typeof(ProjectProfile));
            services.AddScoped<IWalletService, WalletService>();
        }
    }
}
