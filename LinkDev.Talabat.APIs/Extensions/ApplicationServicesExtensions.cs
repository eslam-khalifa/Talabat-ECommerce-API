using LinkDev.Talabat.APIs.Errors;
using LinkDev.Talabat.APIs.Helpers;
using LinkDev.Talabat.Application;
using LinkDev.Talabat.Core.Entities.Identity;
using LinkDev.Talabat.Core.Services.Contracts;
using LinkDev.Talabat.Infrastructure.Data;
using LinkDev.Talabat.Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace LinkDev.Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Customize the validation error response.
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value?.Errors)
                        .Select(x => x.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(IProductService), typeof(ProductService));
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));
            services.AddScoped(typeof(IRoleService), typeof(RoleService));
            services.AddScoped(typeof(IUserService), typeof(UserService));
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddScoped(typeof(IFileService<>), typeof(FileService<>));

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // AddIdentity: Register Identity services (UserManager, SignInManager, RoleManager, PasswordHasher, UserValidators, PasswordValidators, TokenProviders, etc.)
            // AddEntityFrameworkStores: Register IUserStore, IRoleStore, etc. It uses EFCore as the store for identity information.
            // Dependency Injection is for Services and Repositories not for Core Classes. No need to register Core classes because we don't reuse the same instance across the app.
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true; // user must confirm email to login
                options.User.RequireUniqueEmail = true; // Ensure emails are unique
            }).AddEntityFrameworkStores<ApplicationIdentityDbContext>()
            .AddDefaultTokenProviders(); // this is a default token provider for email confirmation, password reset, etc.
            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            // Check Authentication by validating the JWT token sent in the request headers.
            // populate the instance of ClaimsPrincipal which has the User property based on the validated token.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", options => // You need Bearer to contain JWT (Container) and JWT is the content (user claims) inside this container. Bearer: Authentication <Bearer Token>.
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:AuthKey"] ?? string.Empty)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }

        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                var connection = configuration.GetConnectionString("redis");
                return ConnectionMultiplexer.Connect(connection);
            });
            services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            });

            return services;
        }

        public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod()
                        .WithOrigins(configuration["FrontBaseUrl"] ?? string.Empty);
                });
            });
            return services;
        }
    }
}
