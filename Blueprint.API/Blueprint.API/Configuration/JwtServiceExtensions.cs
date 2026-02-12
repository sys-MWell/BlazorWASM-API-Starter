using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Blueprint.API.Configuration
{
    /// <summary>
    /// Extension methods for configuring JWT authentication.
    /// </summary>
    public static class JwtServiceExtensions
    {
        /// <summary>
        /// Configures JWT Bearer authentication using settings from configuration.
        /// The JWT:Key should be stored securely using User Secrets (development) or Azure Key Vault (production).
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        /// <exception cref="InvalidOperationException">Thrown when JWT:Key is not configured.</exception>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger("JwtServiceExtensions");

            logger.LogInformation("Configuring JWT Bearer authentication");

            var jwtSettings = configuration.GetSection("Jwt");
            var issuer = jwtSettings["Issuer"] ?? string.Empty;
            var audience = jwtSettings["Audience"] ?? string.Empty;
            var key = jwtSettings["Key"];

            if (string.IsNullOrWhiteSpace(key))
            {
                logger.LogError("JWT signing key is not configured");
                throw new InvalidOperationException(
                    "JWT signing key is not configured. " +
                    "For local development, use 'dotnet user-secrets set \"Jwt:Key\" \"your-secret-key-min-32-chars\"' " +
                    "or configure via Visual Studio Connected Services > Secrets.json. " +
                    "For production, use Azure Key Vault or environment variables.");
            }

            if (key.Length < 32)
            {
                logger.LogError("JWT signing key is too short. Length: {KeyLength}, Required: 32", key.Length);
                throw new InvalidOperationException(
                    "JWT signing key must be at least 32 characters for security. " +
                    "Current key length: " + key.Length);
            }

            logger.LogDebug("JWT settings - Issuer: {Issuer}, Audience: {Audience}, KeyLength: {KeyLength}", issuer, audience, key.Length);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var eventLogger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtAuthentication");
                        eventLogger.LogWarning("JWT authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var eventLogger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                            .CreateLogger("JwtAuthentication");
                        var username = context.Principal?.Identity?.Name ?? "unknown";
                        eventLogger.LogDebug("JWT token validated for user: {Username}", username);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization();

            logger.LogInformation("JWT Bearer authentication configured successfully");

            return services;
        }
    }
}
