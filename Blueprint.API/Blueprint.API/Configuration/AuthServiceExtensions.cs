using Blueprint.API.Models.Shared;

namespace Blueprint.API.Configuration
{
    /// <summary>
    /// Extension methods for registering database services.
    /// </summary>
    public static class AuthServiceExtensions
    {
        /// <summary>
        /// Registers database connection settings from configuration.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">The application configuration.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("Connection string 'DatabaseConnection' not found.");

            services.AddSingleton(new DatabaseSettings
            {
                AppDbConnectionString = connectionString
            });

            return services;
        }
    }
}
