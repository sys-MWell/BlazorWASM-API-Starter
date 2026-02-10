using Template.Models.Dtos;
using Template.Models.Models;
using Blazor.Web.Domain.Auth;
using Blazor.Web.Domain.Validation;
using Blazor.Web.Repository.User;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Blazor.Web.Logic.User
{
    /// <summary>
    /// Provides application-level user logic for authentication.
    /// </summary>
    public class UserLogic(IUserRepository userRepository, ITokenStore tokenStore, ILogicValidator validator, IUserSession userSession, ILogger<UserLogic> logger) : IUserLogic
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenStore _tokenStore = tokenStore;
        private readonly ILogicValidator _validator = validator;
        private readonly IUserSession _userSession = userSession;
        private readonly ILogger<UserLogic> _logger = logger;

        /// <summary>
        /// Attempts to authenticate a user and persist a session token.
        /// When authentication fails, will inspect a JSON error payload (looking for "error" and numeric "code")
        /// to derive a more specific ErrorMessage/AppErrorCode; otherwise falls back to the original message/code.
        /// </summary>
        /// <returns>
        /// Success with user details; failure with parsed (or fallback) error code and message.
        /// </returns>
        public async Task<ApiResponse<UserDetailDto>> UserLoginAsync(LoginUserDto loginCredentials)
        {
            _logger.LogDebug("Processing login request for user: {Username}", loginCredentials.Username);
            var authResponse = await _userRepository.UserLoginAsync(loginCredentials);

            if (!authResponse.IsSuccess)
            {
                string? finalMessage = authResponse.ErrorMessage;
                AppErrorCode? finalCode = authResponse.ErrorCode;

                if (!string.IsNullOrWhiteSpace(finalMessage) && finalMessage.TrimStart().StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(finalMessage);
                    var root = doc.RootElement;

                    if (root.TryGetProperty("error", out var errorProp))
                    {
                        finalMessage = errorProp.GetString() ?? finalMessage;
                    }

                    if (root.TryGetProperty("code", out var codeProp) && codeProp.ValueKind == JsonValueKind.Number && codeProp.TryGetInt32(out var codeInt))
                    {
                        if (Enum.IsDefined(typeof(AppErrorCode), codeInt))
                        {
                            finalCode = (AppErrorCode)codeInt;
                        }
                    }

                }

                _logger.LogWarning("Login API call failed for user: {Username}, ErrorCode: {ErrorCode}", loginCredentials.Username, finalCode);
                return new ApiResponse<UserDetailDto>
                {
                    IsSuccess = false,
                    ErrorCode = finalCode,
                    ErrorMessage = finalMessage
                };
            }

            var ensured = _validator.EnsureSuccess(authResponse, AppErrorCode.LoginFailed, "Invalid credentials.");
            if (!ensured.IsSuccess)
            {
                _logger.LogWarning("Login validation failed for user: {Username}", loginCredentials.Username);
                return new ApiResponse<UserDetailDto>
                {
                    IsSuccess = false,
                    ErrorCode = ensured.ErrorCode,
                    ErrorMessage = ensured.ErrorMessage
                };
            }

            var token = authResponse.Data?.Token;
            var user = authResponse.Data?.User;
            if (_validator.IsTokenMissing(token) || user is null)
            {
                _logger.LogWarning("Token missing after login for user: {Username}", loginCredentials.Username);
                return _validator.Fail<UserDetailDto>(AppErrorCode.TokenMissing, "Authentication token was not issued.");
            }

            _tokenStore.SetToken(token!);
            if (_tokenStore.IsExpired())
            {
                _logger.LogWarning("Token expired immediately after login for user: {Username}", loginCredentials.Username);
                _tokenStore.ClearToken();
                return _validator.Fail<UserDetailDto>(AppErrorCode.TokenExpired, "Authentication token expired. Please log in again.");
            }

            // Persist full user detail for UI consumption
            _userSession.SetUser(user);
            _logger.LogDebug("Login completed successfully for user: {Username}, UserId: {UserId}", loginCredentials.Username, user.Id);

            return new ApiResponse<UserDetailDto>
            {
                IsSuccess = true,
                Data = user,
                ErrorCode = AppErrorCode.None
            };
        }

        /// <summary>
                /// Registers a new user account (not implemented).
                /// </summary>
                /// <returns>
                /// Not implemented.
                /// </returns>
                public async Task<ApiResponse<UserDetailDto>> UserRegisterAsync(RegisterUserDto registerCredentials)
                {
                    _logger.LogDebug("Processing registration request for user: {Username}", registerCredentials.Username);
                    var authResponse = await _userRepository.UserRegisterAsync(registerCredentials);

                    if (!authResponse.IsSuccess)
                    {
                        string? finalMessage = authResponse.ErrorMessage;
                        AppErrorCode? finalCode = authResponse.ErrorCode;

                        if (!string.IsNullOrWhiteSpace(finalMessage) && finalMessage.TrimStart().StartsWith("{"))
                        {
                            using var doc = JsonDocument.Parse(finalMessage);
                            var root = doc.RootElement;

                            if (root.TryGetProperty("error", out var errorProp))
                            {
                                finalMessage = errorProp.GetString() ?? finalMessage;
                            }

                            if (root.TryGetProperty("code", out var codeProp) && codeProp.ValueKind == JsonValueKind.Number && codeProp.TryGetInt32(out var codeInt))
                            {
                                if (Enum.IsDefined(typeof(AppErrorCode), codeInt))
                                {
                                    finalCode = (AppErrorCode)codeInt;
                                }
                            }
                        }

                        _logger.LogWarning("Registration API call failed for user: {Username}, ErrorCode: {ErrorCode}", registerCredentials.Username, finalCode);
                        return new ApiResponse<UserDetailDto>
                        {
                            IsSuccess = false,
                            ErrorCode = finalCode,
                            ErrorMessage = finalMessage
                        };
                    }

                    _logger.LogDebug("Registration completed successfully for user: {Username}", registerCredentials.Username);
                    return new ApiResponse<UserDetailDto>
                    {
                        IsSuccess = true,
                        ErrorCode = AppErrorCode.None
                    };

                }
            }
        }
