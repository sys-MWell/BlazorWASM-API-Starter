using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Template.Models.Models;
using Blazor.Web.Models.Models;

namespace Blazor.Web.Repository.Shared
{
    /// <summary>
    /// Provides helper functionality to convert <see cref="HttpResponseMessage"/> instances
    /// into strongly typed <see cref="ApiResponse{T}"/> results.
    /// </summary>
    public static class ApiResponseRepoHelper
    {
        public static Task<ApiResponse<T>> HandleHttpResponseAsync<T>(HttpResponseMessage response)
            => HandleHttpResponseAsync<T>(response, logger: null);

        /// <summary>
        /// Processes an <see cref="HttpResponseMessage"/> and constructs an <see cref="ApiResponse{T}"/>.
        /// On success (2xx), attempts to deserialize the JSON payload to <typeparamref name="T"/>.
        /// On failure, populates error details based on the HTTP status code and body content.
        /// </summary>
        /// <typeparam name="T">The target type to deserialize the JSON content into.</typeparam>
        /// <param name="response">The HTTP response to handle.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> indicating success or failure, containing deserialized data when successful,
        /// or error information (message and code) when unsuccessful or deserialization fails.
        /// </returns>
        public static async Task<ApiResponse<T>> HandleHttpResponseAsync<T>(HttpResponseMessage response, ILogger? logger)
        {
            var apiResponse = new ApiResponse<T>();

            logger?.LogInformation(
                "API response received. StatusCode: {StatusCode}",
                (int)response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                apiResponse.IsSuccess = true;
                if (response.Content != null)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        try
                        {
                            apiResponse.Data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        }
                        catch
                        {
                            // Optionally handle deserialization errors
                            apiResponse.IsSuccess = false;
                            apiResponse.ErrorMessage = "Failed to deserialise response.";
                            logger?.LogWarning(
                                "API response deserialization failed. StatusCode: {StatusCode}",
                                (int)response.StatusCode);
                        }
                    }
                }

                if (apiResponse.IsSuccess)
                {
                    logger?.LogInformation("API response handled successfully.");
                }
            }
            else
            {
                apiResponse.IsSuccess = false;
                apiResponse.ErrorCode = (AppErrorCode?)response.StatusCode;
                string errorMessage = response.Content != null ? await response.Content.ReadAsStringAsync() : string.Empty;
                apiResponse.ErrorMessage = string.IsNullOrWhiteSpace(errorMessage)
                    ? (response.ReasonPhrase ?? "API call failed.")
                    : errorMessage;

                logger?.LogWarning(
                    "API request failed. StatusCode: {StatusCode}, ErrorCode: {ErrorCode}",
                    (int)response.StatusCode,
                    apiResponse.ErrorCode);
            }

            return apiResponse;
        }
    }
}