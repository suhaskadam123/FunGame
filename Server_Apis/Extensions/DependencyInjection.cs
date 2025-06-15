using DataAccessLayer.Data;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Server_Apis.Hubs;
using Server_Apis.Interfaces;
using Server_Apis.Services;

namespace Server_Apis.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // SignalR
            services.AddSignalR();

            // Scoped Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWinnerService, WinnerService>();
            
            // Singleton Service
            services.AddSingleton<GlobalVarWinner>();

            // Hosted Services
            services.AddHostedService<MinuteTimerService>();
            

            return services;
        }
    }
}
