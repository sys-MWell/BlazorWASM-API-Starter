using Template.Models.Dtos;
using Blazor.Web.Logic.Services.Validation;
using Blazor.Web.Logic.User;
using Microsoft.Extensions.Logging;

namespace Blazor.Web.Logic.Auth
{
    /// <summary>
    /// Provides authentication services for user login, registration, and logout operations.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICredentialValidator _credentialValidator;
        private readonly IUserLogic _userLogic;
        private readonly ILogger<AuthenticationService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="credentialValidator">The credential validator for validating user input.</param>
        /// <param name="userLogic">The user logic service for authentication operations.</param>
        /// <param name="logger">The logger for recording authentication events.</param>
        public AuthenticationService(ICredentialValidator credentialValidator, IUserLogic userLogic, ILogger<AuthenticationService> logger)
        {
            _credentialValidator = credentialValidator;
            _userLogic = userLogic;
            _logger = logger;
        }

        /// <inheritdoc />
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

            var response = await _userLogic.UserLoginAsync(user);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Login successful for user: {Username}", user.Username);
                result.Success = true;
                return result;
            }

            _logger.LogWarning("Login failed for user: {Username}, ErrorCode: {ErrorCode}", user.Username, response.ErrorCode);
            result.ErrorCode = response.ErrorCode?.ToString();
            result.Errors.Add(response.ErrorMessage ?? "Login failed.");
            return result;
            }

            /// <inheritdoc />
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

            var response = await _userLogic.UserRegisterAsync(user);
            if (response.IsSuccess)
            {
                _logger.LogInformation("Registration successful for user: {Username}", user.Username);
                result.Success = true;
                return result;
            }

            _logger.LogWarning("Registration failed for user: {Username}, ErrorCode: {ErrorCode}", user.Username, response.ErrorCode);
            result.ErrorCode = response.ErrorCode?.ToString();
            result.Errors.Add(response.ErrorMessage ?? "Registration failed.");
            return result;
                }

                /// <inheritdoc />
                public Task LogoutAsync()
                {
                    throw new NotImplementedException();
                }
            }
}
