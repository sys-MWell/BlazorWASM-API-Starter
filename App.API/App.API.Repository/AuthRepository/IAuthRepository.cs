using App.Models.Dtos;
using App.Models.Models;

namespace App.API.Repository.UserRepository
{
    /// <summary>
    /// Defines methods for user-related operations such as retrieval, login, and registration.
    /// </summary>
    public interface IAuthRepository
    {
        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> containing user details if found.</returns>
        Task<ApiResponse<IEnumerable<AuthDetailsDto>>> GetUserByUsername(string username);

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="userLogin">The login credentials of the user.</param>
        /// <returns>An <see cref="ApiResponse{UserLoginResponseDto}"/> containing user details if authentication is successful.</returns>
        Task<ApiResponse<UserLoginResponseDto>> LoginUser(string userLogin);

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="userRegister">The registration details of the new user.</param>
        /// <returns>An <see cref="ApiResponse{AuthDetailsDto}"/> containing user details if registration is successful.</returns>
        Task<ApiResponse<AuthDetailsDto>> RegisterUser(RegisterUserDto userRegister);
    }
}
