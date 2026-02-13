using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Models.Interfaces;
using Microsoft.OpenApi.Models.References;
using Scalar.AspNetCore;

namespace Blueprint.API.Configuration
{
    /// <summary>
    /// Extension methods for configuring OpenAPI documentation.
    /// </summary>
    public static class OpenApiServiceExtensions
    {
        /// <summary>
        /// Configures OpenAPI with JWT Bearer authentication support.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddOpenApiWithJwtAuth(this IServiceCollection services)
        {
            services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Blueprint API",
                        Version = "v1",
                        Description = "Enterprise-grade API with JWT authentication"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes = new Dictionary<string, IOpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                            Name = "Authorization",
                            In = ParameterLocation.Header,
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT"
                        }
                    };

                    document.Security ??= [];
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecuritySchemeReference("Bearer"),
                            new List<string>()
                        }
                    });

                    return Task.CompletedTask;
                });
            });

            return services;
        }

        /// <summary>
        /// Configures the OpenAPI documentation and Scalar UI middleware for development environments.
        /// </summary>
        /// <param name="app">The web application.</param>
        /// <returns>The web application for chaining.</returns>
        public static WebApplication UseOpenApiInDevelopment(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Blueprint API")
                           .WithTheme(ScalarTheme.BluePlanet)
                           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                });
            }

            return app;
        }
    }
}
