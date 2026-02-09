using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Repository.AuthRepository.Commands
{
    /// <summary>
    /// Defines command operations for authentication-related data modifications.
    /// Part of CQRS pattern - handles write operations only.
    /// </summary>
    public interface IAuthCommandRepository
    {
        /// <summary>
        /// Registers a new user using the domain entity.
        /// </summary>
        /// <param name="user">The domain user to register (contains hashed password).</param>
        /// <returns>An <see cref="ApiResponse{UserDetailDto}"/> containing user details if registration is successful.</returns>
        Task<ApiResponse<UserDetailDto>> RegisterUser(User user);
    }
}
