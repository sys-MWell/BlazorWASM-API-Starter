using Blazor.Web.Domain.Shared;

namespace Blazor.Web.Configuration;

/// <summary>
/// Extension methods for configuring API-related services.
/// </summary>
public static class ApiServiceExtensions
{
    /// <summary>
    /// Adds API services including HttpClient configuration and ApiSettings to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration containing API settings.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Server-side HttpClient for calling external/internal APIs
        services.AddHttpClient("ApiClient", client =>
        {
            var apiUrl = configuration.GetValue<string>("AppApi:BaseUrl") ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(apiUrl))
            {
                client.BaseAddress = new Uri(apiUrl);
            }
        });

        // Bind and register ApiSettings for repositories/services that require it
        services.AddSingleton(sp => new ApiSettings
        {
            AppApiBaseUrl = configuration.GetValue<string>("AppApi:BaseUrl") ?? string.Empty
        });

        return services;
    }
}
