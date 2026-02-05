using Blueprint.API.Logic.Helpers;
using Blueprint.API.Repository.UserRepository;
using Template.Models.Dtos;
using Template.Models.Models;
using Microsoft.AspNetCore.Identity;

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
        public async Task<ApiResponse<IEnumerable<UserDetailDto>>> GetUserByUsername(string username)
        {
            var repositoryResponse = await _userRepository.GetUserByUsername(username);

            var mapped = repositoryResponse.Data ?? Array.Empty<UserDetailDto>();

            return ApiResponseLogicHelper.HandleRepositoryResponse(
                new ApiResponse<IEnumerable<UserDetailDto>>
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
        public async Task<ApiResponse<IEnumerable<UserDetailDto>>> LoginUser(LoginUserDto userLogin)
        {
            // Check if user exists
            var existingUser = await GetUserByUsername(userLogin.Username);

            if (existingUser.IsSuccess == false)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("Username does not exist", AppErrorCode.UserNotFound);
            }

            var hashResponse = await _userRepository.GetPasswordHashByUsername(userLogin.Username);
            if (!hashResponse.IsSuccess || string.IsNullOrWhiteSpace(hashResponse.Data))
            {
                return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("Invalid credentials", AppErrorCode.Unauthorized);
            }

            var verified = _passwordVerifier.Verify(userLogin.Username, hashResponse.Data!, userLogin.UserPassword);
            if (!verified)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("Invalid password", AppErrorCode.PasswordInvalid);
            }

            // Fetch user details for successful login
            var user = await _userRepository.GetUserByUsername(userLogin.Username);
            if (!user.IsSuccess || user.Data is null)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("User details returned with faults", AppErrorCode.ServerError);
            }

            var successData = new List<UserDetailDto>
            {
                new UserDetailDto
                {
                    Id = user.Data!.First().Id,
                    Username = user.Data.First().Username,
                    Role = user.Data.First().Role
                }
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

            // Hash the password using the password verifier
            var user = new RegisterUserDto
            {
                Username = userRegister.Username,
                Role = userRegister.Role ?? "User",
                UserPassword = _passwordVerifier.Hash(userRegister.Username, userRegister.UserPassword)
            };

            // Repo call to create user
            var repositoryResponse = await _userRepository.RegisterUser(user);

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
