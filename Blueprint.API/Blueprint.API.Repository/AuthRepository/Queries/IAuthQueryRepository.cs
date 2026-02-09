using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Repository.AuthRepository.Queries
{
    /// <summary>
    /// Defines query operations for authentication-related data retrieval.
    /// Part of CQRS pattern - handles read operations only.
    /// </summary>
    public interface IAuthQueryRepository
    {
        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username of the user to retrieve.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> containing user details if found.</returns>
        Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username);

        /// <summary>
        /// Retrieves the stored password hash for a given username.
        /// </summary>
        /// <param name="username">The username to look up.</param>
        /// <returns>An <see cref="ApiResponse{String}"/> containing the stored password hash.</returns>
        Task<ApiResponse<string>> GetPasswordHashByUsername(string username);
    }
}
