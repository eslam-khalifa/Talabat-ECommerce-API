using Hangfire;
using LinkDev.Talabat.APIs.Extensions;
using LinkDev.Talabat.APIs.Middlewares;
using LinkDev.Talabat.Application;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Infrastructure.Data;
using LinkDev.Talabat.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Talabat.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var webApplicationBuilder = WebApplication.CreateBuilder(args);

            #region Add services to the container.

            webApplicationBuilder.Services.AddPresentation(webApplicationBuilder.Configuration);
            webApplicationBuilder.Services.AddApplication(webApplicationBuilder.Configuration);
            webApplicationBuilder.Services.AddInfrastructure(webApplicationBuilder.Configuration);

            #endregion

            var app = webApplicationBuilder.Build();

            #region Apply migrations and seed data.

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var dbContext = services.GetRequiredService<StoreDbContext>();
            var applicationIdentityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

            await dbContext.Database.MigrateAsync();
            await applicationIdentityDbContext.Database.MigrateAsync();

            await ApplicationIdentityDbContextSeed.SeedAsync(userManager, roleManager);
            await StoreDbContextSeed.SeedAsync(dbContext, userManager);

            #endregion

            #region Configure the HTTP request pipeline.

            app.UseMiddleware<ExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddlewares();
            }
            app.UseStatusCodePagesWithReExecute("/errors/{0}"); // Handle error status codes.
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.UseHangfireDashboard("/hangfire"); // Hangfire dashboard to monitor jobs
            
            // Schedule recurring job to clean up expired OTPs every hour
            RecurringJob.AddOrUpdate<OtpCleanupService>(
                "cleanup-expired-otps", // Job name
                service => service.RemoveExpiredOtpsAsync(CancellationToken.None), // Job method
                Cron.Hourly); // Job schedule

            // Provides Premade Identity endpoints (/login, /register, /logout, /change-password, /reset-password, /confirm-email, ...)
            // It uses Minimal APIs to provide these endpoints.
            // MapGroup is used to group these endpoints under a common route prefix.
            //app.MapGroup("api/account").MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}
