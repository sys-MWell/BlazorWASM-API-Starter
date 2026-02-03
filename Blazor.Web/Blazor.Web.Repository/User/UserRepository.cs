using Template.Models.Dtos;
using Template.Models.Models;
using Blazor.Web.Domain.Shared;
using Blazor.Web.Repository.Shared;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace Blazor.Web.Repository.User
{
    /// <summary>
    /// Repository responsible for user-related API operations such as login and registration.
    /// Implements infrastructure concerns by communicating with the backend via <see cref="HttpClient"/>.
    /// </summary>
    public class UserRepository(IHttpClientFactory httpClientFactory, ILogger<UserRepository> logger, ApiSettings settings) : IUserRepository
    {
        private readonly HttpClient httpClient = httpClientFactory.CreateClient("ApiClient");
        private readonly ILogger<UserRepository> logger = logger;
        private readonly ApiSettings settings = settings;

        /// <summary>
        /// Attempts to authenticate a user using the provided login credentials.
        /// </summary>
        /// <param name="loginCredentials">The username and password combination supplied by the user.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing an <see cref="AuthResponseDto"/> on success
        /// (including JWT token and user detail information); otherwise an error response with
        /// <see cref="ApiResponse{T}.ErrorMessage"/> and optional <see cref="ApiResponse{T}.ErrorCode"/>.
        /// </returns>
        /// <remarks>
        /// Sends a POST request to <c>AppAPI.Users.Login</c>. The response is processed through
        /// <see cref="ApiResponseRepoHelper.HandleHttpResponseAsync{T}(HttpResponseMessage)"/>.
        /// </remarks>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during the HTTP request.</exception>
        public async Task<ApiResponse<AuthResponseDto>> UserLoginAsync(LoginUserDto loginCredentials)
        {
            try
            {
                var url = $"{this.settings.AppApiBaseUrl}{ApiSettings.Auth.Login}";
                var response = await this.httpClient.PostAsJsonAsync(url, loginCredentials);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    this.logger.LogWarning("Login failed with status {StatusCode} error: {Error}", response.StatusCode, error);
                }
                return await ApiResponseRepoHelper.HandleHttpResponseAsync<AuthResponseDto>(response);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during user login API call");
                throw;
            }
        }

        /// <summary>
        /// Attempts to register a new user using the provided registration credentials.
        /// </summary>
        /// <param name="registercredentials">The user information required for registration (email, password, etc.).</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing an <see cref="AuthResponseDto"/> on success
        /// (including JWT token and user detail information); otherwise an error response with
        /// <see cref="ApiResponse{T}.ErrorMessage"/> and optional <see cref="ApiResponse{T}.ErrorCode"/>.
        /// </returns>
        /// <remarks>
        /// Sends a POST request to <c>AppAPI.Auth.Register</c>. The response is processed through
        /// <see cref="ApiResponseRepoHelper.HandleHttpResponseAsync{T}(HttpResponseMessage)"/>.
        /// </remarks>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during the HTTP request.</exception>
        public async Task<ApiResponse<AuthResponseDto>> UserRegisterAsync(RegisterUserDto registercredentials)
        {
            try
            {
                var url = $"{this.settings.AppApiBaseUrl}{ApiSettings.Auth.Register}";
                var response = await this.httpClient.PostAsJsonAsync(url, registercredentials);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    this.logger.LogWarning("register failed with status {StatusCode} error: {Error}", response.StatusCode, error);
                }
                return await ApiResponseRepoHelper.HandleHttpResponseAsync<AuthResponseDto>(response);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during user registration API call");
                throw;
            }
        }
    }
}
