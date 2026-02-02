using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Logic.UserLogic
{
    /// <summary>
    /// Provides user-related logic such as registration, login, and retrieval by username.
    /// </summary>
    public interface IUserLogic
    {
        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="userRegister">The registration details for the new user.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing the registered user's details if successful.
        /// </returns>
        Task<ApiResponse<UserDetailDto>> RegisterUser(RegisterUserDto userRegister);

        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="userLogin">The login credentials of the user.</param>
        /// <returns>
        /// An <see cref="ApiResponse{UserDetailDto}"/> containing the user's details if authentication is successful.
        /// </returns>
        Task<ApiResponse<IEnumerable<UserDetailDto>>> LoginUser(LoginUserDto userLogin);

        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>
        /// An <see cref="ApiResponse{UserDetailDto}"/> containing the user's details if found.
        /// </returns>
        Task<ApiResponse<IEnumerable<UserDetailDto>>> GetUserByUsername(string username);
    }
}
