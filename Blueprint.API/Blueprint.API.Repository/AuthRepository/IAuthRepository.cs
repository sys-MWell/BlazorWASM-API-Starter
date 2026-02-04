using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Repository.UserRepository
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
        Task<ApiResponse<IEnumerable<UserDetailDto>>> GetUserByUsername(string username);


        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="userRegister">The registration details of the new user.</param>
        /// <returns>An <see cref="ApiResponse{UserDetailDto}"/> containing user details if registration is successful.</returns>
        Task<ApiResponse<UserDetailDto>> RegisterUser(RegisterUserDto userRegister);

        /// <summary>
        /// Retrieves the stored password hash for a given username.
        /// </summary>
        /// <param name="username">The username to look up.</param>
        /// <returns>An <see cref="ApiResponse{String}"/> containing the stored password hash.</returns>
        Task<ApiResponse<string>> GetPasswordHashByUsername(string username);
    }
}
