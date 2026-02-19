using Template.Models.Dtos;
using Template.Models.Models;
using Blazor.Web.Domain.Auth;
using Blazor.Web.Domain.Validation;
using Blazor.Web.Logic.Services.Validation;
using Blazor.Web.Repository.User;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Blazor.Web.Logic.Auth
{
    /// <summary>
    /// Provides authentication services for user login, registration, and logout operations.
    /// Handles credential validation, API communication, token management, and session persistence.
    /// </summary>
    /// <remarks>
    /// This service follows the Single Responsibility Principle by focusing solely on authentication concerns.
    /// It delegates:
    /// <list type="bullet">
    ///   <item><description>API communication to <see cref="IUserRepository"/></description></item>
    ///   <item><description>Credential validation to <see cref="ICredentialValidator"/></description></item>
    ///   <item><description>Token storage to <see cref="ITokenStore"/></description></item>
    ///   <item><description>Session management to <see cref="IUserSession"/></description></item>
    /// </list>
    /// </remarks>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICredentialValidator _credentialValidator;
        private readonly ITokenStore _tokenStore;
        private readonly ILogicValidator _validator;
        private readonly IUserSession _userSession;
        private readonly ILogger<AuthenticationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="userRepository">Repository for user API operations.</param>
        /// <param name="credentialValidator">Validator for user credentials.</param>
        /// <param name="tokenStore">Store for managing authentication tokens.</param>
        /// <param name="validator">Generic logic validation helper.</param>
        /// <param name="userSession">Session manager for user state.</param>
        /// <param name="logger">Logger for authentication events.</param>
        public AuthenticationService(
            IUserRepository userRepository,
            ICredentialValidator credentialValidator,
            ITokenStore tokenStore,
            ILogicValidator validator,
            IUserSession userSession,
            ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _credentialValidator = credentialValidator;
            _tokenStore = tokenStore;
            _validator = validator;
            _userSession = userSession;
            _logger = logger;
        }

        /// <summary>
        /// Attempts to authenticate a user with the provided login credentials.
        /// </summary>
        /// <param name="user">The user credentials required for login.</param>
        /// <returns>
        /// An <see cref="AuthenticationResult"/> indicating success or failure, with
        /// any associated error messages and error code when applicable.
        /// </returns>
        public async Task<AuthenticationResult> LoginAsync(LoginUserDto user)
        {
            _logger.LogInformation("Login attempt for user: {Username}", user.Username);
            var result = new AuthenticationResult();

            var validationResult = _credentialValidator.ValidateCredentials(user.Username, user.UserPassword, forLogin: true);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Login validation failed for user: {Username}", user.Username);
                result.Errors = validationResult.Errors.Select(e => e.Message).ToList();
                return result;
            }

            var authResponse = await _userRepository.UserLoginAsync(user);

            if (!authResponse.IsSuccess)
            {
                var (errorMessage, errorCode) = ParseErrorResponse(authResponse.ErrorMessage, authResponse.ErrorCode);
                _logger.LogWarning("Login API call failed for user: {Username}, ErrorCode: {ErrorCode}", user.Username, errorCode);
                result.ErrorCode = errorCode?.ToString();
                result.Errors.Add(errorMessage ?? "Login failed.");
                return result;
            }

            var ensured = _validator.EnsureSuccess(authResponse, AppErrorCode.LoginFailed, "Invalid credentials.");
            if (!ensured.IsSuccess)
            {
                _logger.LogWarning("Login validation failed for user: {Username}", user.Username);
                result.ErrorCode = ensured.ErrorCode?.ToString();
                result.Errors.Add(ensured.ErrorMessage ?? "Login failed.");
                return result;
            }

            var token = authResponse.Data?.Token;
            var userDetail = authResponse.Data?.User;

            if (_validator.IsTokenMissing(token) || userDetail is null)
            {
                _logger.LogWarning("Token missing after login for user: {Username}", user.Username);
                result.ErrorCode = AppErrorCode.TokenMissing.ToString();
                result.Errors.Add("Authentication token was not issued.");
                return result;
            }

            _tokenStore.SetToken(token!);
            if (_tokenStore.IsExpired())
            {
                _logger.LogWarning("Token expired immediately after login for user: {Username}", user.Username);
                _tokenStore.ClearToken();
                result.ErrorCode = AppErrorCode.TokenExpired.ToString();
                result.Errors.Add("Authentication token expired. Please log in again.");
                return result;
            }

            _userSession.SetUser(userDetail);
            _logger.LogInformation("Login successful for user: {Username}, UserId: {UserId}", user.Username, userDetail.Id);
            result.Success = true;
            return result;
        }


        /// <summary>
        /// Attempts to register a new user with the provided registration details.
        /// </summary>
        /// <param name="user">The user credentials required for registration.</param>
        /// <returns>
        /// An <see cref="AuthenticationResult"/> indicating success or failure, with
        /// any associated error messages and error code when applicable.
        /// </returns>
        public async Task<AuthenticationResult> RegisterAsync(RegisterUserDto user)
        {
            _logger.LogInformation("Registration attempt for user: {Username}", user.Username);
            var result = new AuthenticationResult();
            user.Username = user.Username?.Trim() ?? string.Empty;

            var validationResult = _credentialValidator.ValidateCredentials(user.Username, user.UserPassword, forLogin: false);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Registration validation failed for user: {Username}", user.Username);
                result.Errors = validationResult.Errors.Select(e => e.Message).ToList();
                return result;
            }

            var authResponse = await _userRepository.UserRegisterAsync(user);

            if (!authResponse.IsSuccess)
            {
                var (errorMessage, errorCode) = ParseErrorResponse(authResponse.ErrorMessage, authResponse.ErrorCode);
                _logger.LogWarning("Registration API call failed for user: {Username}, ErrorCode: {ErrorCode}", user.Username, errorCode);
                result.ErrorCode = errorCode?.ToString();
                result.Errors.Add(errorMessage ?? "Registration failed.");
                return result;
            }

            _logger.LogInformation("Registration successful for user: {Username}", user.Username);
            result.Success = true;
            return result;
        }

        /// <summary>
        /// Logs out the current user by clearing authentication tokens and session state.
        /// </summary>
        /// <returns>A completed task when logout processing has finished.</returns>
        public Task LogoutAsync()
        {
            _logger.LogInformation("Logout requested");
            _tokenStore.ClearToken();
            _userSession.Clear();
            _logger.LogInformation("Logout completed");
            return Task.CompletedTask;
        }

        /// <summary>
        /// Parses a JSON error payload from API responses to extract error message and code.
        /// </summary>
        /// <param name="errorMessage">The raw error message, potentially JSON.</param>
        /// <param name="errorCode">The default error code.</param>
        /// <returns>A tuple containing the parsed error message and error code.</returns>
        private static (string? ErrorMessage, AppErrorCode? ErrorCode) ParseErrorResponse(string? errorMessage, AppErrorCode? errorCode)
        {
            if (string.IsNullOrWhiteSpace(errorMessage) || !errorMessage.TrimStart().StartsWith('{'))
            {
                return (errorMessage, errorCode);
            }

            using var doc = JsonDocument.Parse(errorMessage);
            var root = doc.RootElement;

            if (root.TryGetProperty("error", out var errorProp))
            {
                errorMessage = errorProp.GetString() ?? errorMessage;
            }

            if (root.TryGetProperty("code", out var codeProp) &&
                codeProp.ValueKind == JsonValueKind.Number &&
                codeProp.TryGetInt32(out var codeInt) &&
                Enum.IsDefined(typeof(AppErrorCode), codeInt))
            {
                errorCode = (AppErrorCode)codeInt;
            }

            return (errorMessage, errorCode);
        }
    }
}
