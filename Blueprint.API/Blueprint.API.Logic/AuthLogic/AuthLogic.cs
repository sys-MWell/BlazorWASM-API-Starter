using Blueprint.API.Logic.Helpers;
using Blueprint.API.Repository.UserRepository;
using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Logic.UserLogic
{
    /// <summary>
    /// Implements authentication logic including registration, login, and user retrieval.
    /// </summary>
    public class AuthLogic(IAuthRepository userRepository, IPasswordVerifier passwordVerifier) : IAuthLogic
    {
        private readonly IAuthRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        private readonly IPasswordVerifier _passwordVerifier = passwordVerifier ?? throw new ArgumentNullException(nameof(passwordVerifier));

        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing the user's details if found.
        /// </returns>
        public async Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username)
        {
            var repositoryResponse = await _userRepository.GetUserByUsername(username);

            // Fix: Use object initializer with required Id property set to 0 (or another default value)
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
        public async Task<ApiResponse<UserDetailDto> LoginUser(LoginUserDto userLogin)
        {
            // Check if user exists
            var existingUser = await GetUserByUsername(userLogin.Username);

            if (existingUser.IsSuccess == false)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("Username does not exist", AppErrorCode.UserNotFound);
            }

            var hashResponse = await _userRepository.GetPasswordHashByUsername(userLogin.Username);
            if (!hashResponse.IsSuccess || string.IsNullOrWhiteSpace(hashResponse.Data))
            {
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("Invalid credentials", AppErrorCode.Unauthorized);
            }

            var verified = _passwordVerifier.Verify(userLogin.Username, hashResponse.Data!, userLogin.UserPassword);
            if (!verified)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("Invalid password", AppErrorCode.PasswordInvalid);
            }

            if (!existingUser.IsSuccess || existingUser.Data is null)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>("User details returned with faults", AppErrorCode.ServerError);
            }

            var successData = new UserDetailDto
            {
                Id = existingUser.Data!.Id,
                Username = existingUser.Data.Username,
                Role = existingUser.Data.Role
            };

            // Reuse existingUser's status to craft final response
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
            // Username checks
            if (string.IsNullOrWhiteSpace(userRegister.Username) || userRegister.Username.Length < 3)
                return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Invalid username", ErrorCode = AppErrorCode.Validation };

            // Password checks (example: min 8 chars, at least one number)
            if (string.IsNullOrWhiteSpace(userRegister.UserPassword) || userRegister.UserPassword.Length < 8)
                return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Password too weak", ErrorCode = AppErrorCode.Validation };

            // Check if user exists
            var existingUser = await GetUserByUsername(userRegister.Username);

            if (existingUser.IsSuccess)
            {
                return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "User already exists", ErrorCode = AppErrorCode.UserAlreadyExists };
            }

            // Hash the password and prepare repository DTO
            var hashedPassword = _passwordVerifier.Hash(userRegister.Username, userRegister.UserPassword);
            var domainUser = new User
            {
                Username = userRegister.Username,
                Role = "User",
                UserPassword = hashedPassword
            };

            // Repo call to create user
            var repositoryResponse = await _userRepository.RegisterUser(domainUser);

            if (repositoryResponse.IsSuccess == false)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<UserDetailDto>(
                    repositoryResponse.ErrorMessage ?? "An error occurred",
                    repositoryResponse.ErrorCode ?? AppErrorCode.ServerError
                );
            }
            else if (repositoryResponse.IsSuccess)
            {
                // Repository already returns UserDetailDto
                return new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = repositoryResponse.Data!,
                    ErrorCode = AppErrorCode.None
                };
            }

            return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Unexpected error occurred", ErrorCode = AppErrorCode.ServerError };
        }
    }
}
