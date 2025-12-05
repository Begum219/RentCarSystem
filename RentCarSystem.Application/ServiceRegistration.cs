using Microsoft.Extensions.DependencyInjection;
using RentCarSystem.Application.Common.Interfaces;
using RentCarSystem.Application.Services;
using System.Reflection;

namespace RentCarSystem.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Services (14 adet)
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReservationService, ReservationService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IInsuranceService, InsuranceService>();
            services.AddScoped<IVehicleImageService, VehicleImageService>();
            services.AddScoped<IRentalAgreementService, RentalAgreementService>();
            services.AddScoped<IDamageReportService, DamageReportService>();
            services.AddScoped<IDamageImageService, DamageImageService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITwoFactorService, TwoFactorService>();

            return services;
        }
    }
}