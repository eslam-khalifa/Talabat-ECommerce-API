using Microsoft.OpenApi.Models;

namespace LinkDev.Talabat.APIs.Extensions
{
    public static class SwaggerServicesExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer(); // Used to generate Documentation for minimal APIs
            // Configure Swagger Documentation
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName); // To avoid duplicate schema IDs
                c.AddSecurityDefinition("Bearer Authentication", new OpenApiSecurityScheme // Bearer is the security schema name. Choose any name you want.
                {
                    Type = SecuritySchemeType.Http, // it will prepend 'Bearer ' to the token unlike ApiKey.
                    Scheme = "bearer", // the type of the http authentication. only useful for Http type not ApiKey, OAuth2, OpenIdConnect
                    In = ParameterLocation.Header, // Bearer token will be in the header
                    Name = "Authorization", // Name of the header
                    Description = "Please enter your JWT here. Swagger will prepend 'Bearer ' to the token."
                });
                // to apply security globally to all endpoints
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer Authentication" // Should match the name of the security scheme
                            }
                        },
                        new List<string>() // List of scopes used for OAuth2
                    }
                });
            });
            return services;
        }

        public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
        {
            app.UseDeveloperExceptionPage(); // Detailed error pages for development appearing in the Console
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
