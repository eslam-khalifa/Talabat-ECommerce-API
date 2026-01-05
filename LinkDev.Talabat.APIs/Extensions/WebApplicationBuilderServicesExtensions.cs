using Hangfire;
using LinkDev.Talabat.Application;
using LinkDev.Talabat.Core;
using LinkDev.Talabat.Core.Repositories.Contracts;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Infrastructure;
using LinkDev.Talabat.Infrastructure.BasketRepository;
using LinkDev.Talabat.Infrastructure.Data.GenericRepository;
using LinkDev.Talabat.Infrastructure.Identity.IdentityGenericRepository;
using LinkDev.Talabat.Infrastructure.Services;
using LinkDev.Talabat.Infrastructure.Settings;
using Newtonsoft.Json;

namespace LinkDev.Talabat.APIs.Extensions
{
    public static class WebApplicationBuilderServicesExtensions
    { 
        public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddSwaggerServices();
            services.AddCorsServices(configuration);
            // Configure Hangfire to use your SQL Server
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(configuration.GetConnectionString("IdentityConnection"));
            });
            // Add Hangfire server to process jobs
            services.AddHangfireServer();
            // Register your OTP cleanup service
            services.AddScoped<OtpCleanupService>();
            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices(configuration);
            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailProvider"));
            services.AddScoped(typeof(IStoreGenericRepository<>), typeof(StoreGenericRepository<>));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            services.AddScoped(typeof(IIdentityGenericRepository<>), typeof(IdentityGenericRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped(typeof(IEmailService), typeof(SmtpEmailService));
            services.AddDatabaseServices(configuration);
            services.AddAuthenticationServices(configuration);
            return services;
        }
    }
}
