using Blueprint.API.Logic.Helpers;
using Blueprint.API.Repository.AuthRepository.Commands;
using Blueprint.API.Repository.AuthRepository.Queries;
using Microsoft.Extensions.Logging;
using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Logic.UserLogic
{
    /// <summary>
    /// Implements authentication logic including registration, login, and user retrieval.
    /// Uses CQRS pattern with separate query and command repositories.
    /// </summary>
    public class AuthLogic(
        IAuthQueryRepository queryRepository,
        IAuthCommandRepository commandRepository,
        IPasswordVerifier passwordVerifier,
        ILogger<AuthLogic> logger) : IAuthLogic
    {
        private readonly IAuthQueryRepository _queryRepository = queryRepository ?? throw new ArgumentNullException(nameof(queryRepository));
        private readonly IAuthCommandRepository _commandRepository = commandRepository ?? throw new ArgumentNullException(nameof(commandRepository));
        private readonly IPasswordVerifier _passwordVerifier = passwordVerifier ?? throw new ArgumentNullException(nameof(passwordVerifier));
        private readonly ILogger<AuthLogic> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing the user's details if found.
        /// </returns>
        public async Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username)
        {
            _logger.LogDebug("Retrieving user by username: {Username}", username);

            var repositoryResponse = await _queryRepository.GetUserByUsername(username);

            if (!repositoryResponse.IsSuccess)
            {
                _logger.LogDebug("User not found: {Username}", username);
            }

            var mapped = repositoryResponse.Data ?? new UserDetailDto { Id = 0, Username = string.Empty, Role = null };

            return ApiResponseLogicHelper.HandleRepositoryResponse(
                new ApiResponse<UserDetailDto>
                {
                    IsSuccess = repositoryResponse.IsSuccess,
                    ErrorMessage = repositoryResponse.ErrorMessage,
                    ErrorCode = repositoryResponse.ErrorCode,
                    Data = mapped
                },
                mapped
            );
        }

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="userLogin">The login credentials of the user.</param>
        /// <returns>
        /// An <see cref="ApiResponse{UserDetailDto}"/> containing the user's details if authentication is successful.
        /// </returns>
        public async Task<ApiResponse<UserDetailDto>> LoginUser(LoginUserDto userLogin)
        {
            _logger.LogDebug("Login attempt for username: {Username}", userLogin.Username);

            var existingUser = await GetUserByUsername(userLogin.Username);

            if (existingUser.IsSuccess == false)
            {
                _logger.LogDebug("Login failed - username does not exist: {Username}", userLogin.Username);
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("Username does not exist", AppErrorCode.UserNotFound);
            }

            var hashResponse = await _queryRepository.GetPasswordHashByUsername(userLogin.Username);
            if (!hashResponse.IsSuccess || string.IsNullOrWhiteSpace(hashResponse.Data))
            {
                _logger.LogWarning("Login failed - unable to retrieve password hash for username: {Username}", userLogin.Username);
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("Invalid credentials", AppErrorCode.Unauthorized);
            }

            var verified = _passwordVerifier.Verify(userLogin.Username, hashResponse.Data!, userLogin.UserPassword);
            if (!verified)
            {
                _logger.LogDebug("Login failed - invalid password for username: {Username}", userLogin.Username);
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("Invalid password", AppErrorCode.PasswordInvalid);
            }

            if (!existingUser.IsSuccess || existingUser.Data is null)
            {
                _logger.LogError("Login failed - user details returned with faults for username: {Username}", userLogin.Username);
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("User details returned with faults", AppErrorCode.ServerError);
            }

            _logger.LogDebug("Login successful for username: {Username}", userLogin.Username);

            var successData = new UserDetailDto
            {
                Id = existingUser.Data!.Id,
                Username = existingUser.Data.Username,
                Role = existingUser.Data.Role
            };

            return ApiResponseLogicHelper.HandleRepositoryResponse(existingUser, successData);
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="userRegister">The registration details for the new user.</param>
        /// <returns>
        /// An <see cref="ApiResponse{UserDetailDto}"/> containing the registered user's details if successful.
        /// </returns>
        public async Task<ApiResponse<UserDetailDto>> RegisterUser(RegisterUserDto userRegister)
        {
            _logger.LogDebug("Registration attempt for username: {Username}", userRegister.Username);

            if (string.IsNullOrWhiteSpace(userRegister.Username) || userRegister.Username.Length < 3)
            {
                _logger.LogDebug("Registration failed - invalid username: {Username}", userRegister.Username);
                return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Invalid username", ErrorCode = AppErrorCode.Validation };
            }

            if (string.IsNullOrWhiteSpace(userRegister.UserPassword) || userRegister.UserPassword.Length < 8)
            {
                _logger.LogDebug("Registration failed - password too weak for username: {Username}", userRegister.Username);
                return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Password too weak", ErrorCode = AppErrorCode.Validation };
            }

            var existingUser = await GetUserByUsername(userRegister.Username);

            if (existingUser.IsSuccess)
            {
                _logger.LogDebug("Registration failed - user already exists: {Username}", userRegister.Username);
                return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "User already exists", ErrorCode = AppErrorCode.UserAlreadyExists };
            }

            _logger.LogDebug("Hashing password for username: {Username}", userRegister.Username);
            var hashedPassword = _passwordVerifier.Hash(userRegister.Username, userRegister.UserPassword);
            var domainUser = new User
            {
                Username = userRegister.Username,
                Role = "User",
                UserPassword = hashedPassword
            };

            _logger.LogDebug("Calling repository to register user: {Username}", userRegister.Username);
            var repositoryResponse = await _commandRepository.RegisterUser(domainUser);

            if (repositoryResponse.IsSuccess == false)
            {
                _logger.LogWarning("Registration failed at repository level for username: {Username}. Error: {ErrorMessage}", userRegister.Username, repositoryResponse.ErrorMessage);
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>(
                    repositoryResponse.ErrorMessage ?? "An error occurred",
                    repositoryResponse.ErrorCode ?? AppErrorCode.ServerError
                );
            }
            else if (repositoryResponse.IsSuccess)
            {
                _logger.LogDebug("Registration successful for username: {Username}", userRegister.Username);
                return new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = repositoryResponse.Data!,
                    ErrorCode = AppErrorCode.None
                };
            }

            _logger.LogError("Registration failed with unexpected error for username: {Username}", userRegister.Username);
            return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Unexpected error occurred", ErrorCode = AppErrorCode.ServerError };
        }
    }
}
