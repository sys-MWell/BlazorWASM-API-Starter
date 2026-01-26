using Blazor.Web.Domain.Shared;
using Blazor.Web.Models.Models;
using Microsoft.Extensions.Http;
using System.Net.Http;

namespace Blazor.Web.Repository.Shared
{
    /// <summary>
    /// Base repository providing common HTTP functionality for calling the App API.
    /// </summary>
    public class RepositoryBase
    {
        /// <summary>
        /// The configured <see cref="HttpClient"/> instance used for API requests.
        /// </summary>
        protected readonly HttpClient _httpClient;

        /// <summary>
        /// Strongly typed API settings containing base URLs and route segments.
        /// </summary>
        protected readonly ApiSettings _apiSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        /// <param name="httpClientFactory">Factory used to create named <see cref="HttpClient"/> instances.</param>
        /// <param name="apiSettings">Application API configuration settings.</param>
        public RepositoryBase(IHttpClientFactory httpClientFactory, ApiSettings apiSettings)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _apiSettings = apiSettings;
        }

        /// <summary>
        /// Performs a GET request against the App API for the specified relative endpoint.
        /// </summary>
        /// <param name="endpoint">The relative API endpoint (e.g. "api/User/login").</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the raw response body as a string.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown when the response status is not successful.</exception>
        protected async Task<string> GetFromApiAsync(string endpoint)
        {
            var baseUrl = _apiSettings.AppApiBaseUrl;
            var fullUrl = $"{baseUrl}{endpoint}";
            var response = await _httpClient.GetAsync(fullUrl);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}