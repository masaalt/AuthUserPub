using AuthUser.Application.Interfaces.Services;
using AuthUser.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthUser.Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration _config)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IPasswordService, PasswordService>();
        }
    }
}