using Blueprint.API.Logic.Helpers;
using Blueprint.API.Repository.UserRepository;
using Template.Models.Dtos;
using Template.Models.Models;
using Microsoft.AspNetCore.Identity;

namespace Blueprint.API.Logic.UserLogic
{
    public class UserLogic(IAuthRepository userRepository) : IUserLogic
    {
        private readonly IAuthRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

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

            // Map AuthDetailsDto -> UserDetailDto
            var mapped = (repositoryResponse.Data ?? Array.Empty<AuthDetailsDto>())
                .Select(a => new UserDetailDto { Id = a.UserId, Username = a.Username, Role = a.Role })
                .ToList();

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

            // Validate user credentials
            var user = await _userRepository.LoginUser(userLogin.Username);

            if (user.IsSuccess == false)
            {
                return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("Invalid password", AppErrorCode.Unauthorized);
            }
            else if (user.IsSuccess)
            {
                // Since passwords are hashed, use PasswordHasher to verify
                var passwordHasher = new PasswordHasher<string>();
                var hashedPassword = user.Data?.UserPassword;
                if (string.IsNullOrEmpty(hashedPassword))
                {
                    return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("User password not found", AppErrorCode.ServerError);
                }
                var verificationResult = passwordHasher.VerifyHashedPassword(userLogin.Username, hashedPassword, userLogin.UserPassword);
                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("Invalid password", AppErrorCode.PasswordInvalid);
                }
            }
            else
            {
                return ApiResponseLogicHelper.CreateErrorResponse<IEnumerable<UserDetailDto>>("User details returned with faults", AppErrorCode.ServerError);
            }

            var successData = new List<UserDetailDto>
            {
                new UserDetailDto
                {
                    Id = user.Data!.UserId,
                    Username = user.Data.Username,
                    Role = user.Data.Role
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

            // Hash the password
            var passwordHasher = new PasswordHasher<string>();
            var user = new RegisterUserDto
            {
                Username = userRegister.Username,
                Role = userRegister.Role ?? "User",
                UserPassword = passwordHasher.HashPassword(userRegister.Username, userRegister.UserPassword)
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
                var successData = repositoryResponse.Data is null
                    ? null
                    : new UserDetailDto
                    {
                        Id = repositoryResponse.Data.UserId,
                        Username = repositoryResponse.Data.Username,
                        Role = repositoryResponse.Data.Role
                    };

                // Manually construct the response with mapped type
                return new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = successData!,
                    ErrorCode = AppErrorCode.None
                };
            }

            return new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorMessage = "Unexpected error occurred", ErrorCode = AppErrorCode.ServerError };
        }
    }
}
