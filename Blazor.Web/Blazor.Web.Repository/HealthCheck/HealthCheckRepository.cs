using System.Text.Json;
using Blazor.Web.Domain.Shared;
using Blazor.Web.Repository.Shared;

namespace Blazor.Web.Repository.HealthCheck
{
    public class HealthCheck : RepositoryBase, IHealthCheckRepository
    {
        public HealthCheck(IHttpClientFactory httpClientFactory, ApiSettings apiSettings)
            : base(httpClientFactory, apiSettings)
        {
        }

        public async Task<Blazor.Web.Models.Models.HealthCheck> ApiHealthCheck()
        {
            // Use the base method to get the API response as a string
            var json = await GetFromApiAsync("HealthCheck");

            // Deserialize the JSON response to the HealthCheck model
            var healthCheck = JsonSerializer.Deserialize<Blazor.Web.Models.Models.HealthCheck>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (healthCheck == null)
                throw new InvalidOperationException("API did not return valid health check data.");

            return healthCheck;
        }
    }
}