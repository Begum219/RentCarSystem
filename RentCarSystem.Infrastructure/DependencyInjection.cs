using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RentCarSystem.Infrastructure.Context;
using RentCarSystem.Infrastructure.Security;

namespace RentCarSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ✅ AES ENCRYPTION SERVICE (İLK ÖNCE!)
            services.AddSingleton<IAesEncryptionService>(sp =>
            {
                var key = configuration["Encryption:Key"]
                    ?? throw new InvalidOperationException("Encryption:Key not found in appsettings.json");
                var iv = configuration["Encryption:IV"]
                    ?? throw new InvalidOperationException("Encryption:IV not found in appsettings.json");
                return new AesEncryptionService(key, iv);
            });

            // ✅ APPLICATION DB CONTEXT
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            return services;
        }
    }
}