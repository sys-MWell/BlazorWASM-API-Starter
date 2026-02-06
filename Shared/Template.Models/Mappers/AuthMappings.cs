using Template.Models.Dtos;
using Template.Models.Models;

namespace Template.Models
{
    /// <summary>
    /// Mapping helpers to convert between DTOs and domain models.
    /// Keep DTOs minimal and enforce domain rules in services/entities.
    /// </summary>
    public static class AuthMappings
    {
        /// <summary>
        /// Converts a <see cref="RegisterUserDto"/> to a <see cref="User"/> domain model.
        /// </summary>
        /// <param name="dto">The registration DTO to convert.</param>
        /// <param name="passwordHash">The hashed password to assign.</param>
        /// <returns>A <see cref="User"/> domain model.</returns>
        public static User ToDomainUser(this RegisterUserDto dto, string passwordHash)
        {
            return new User
            {
                Username = dto.Username,
                Role = "User",
                UserPassword = passwordHash
            };
        }

        /// <summary>
        /// Converts a <see cref="User"/> domain model to a <see cref="RegisterUserDto"/>.
        /// </summary>
        /// <param name="user">The user domain model to convert.</param>
        /// <returns>A <see cref="RegisterUserDto"/> containing the user's username and password.</returns>
        public static RegisterUserDto ToRegisterDto(this User user)
        {
            return new RegisterUserDto
            {
                Username = user.Username,
                UserPassword = user.UserPassword
            };
        }

        /// <summary>
        /// Converts a <see cref="User"/> domain model to a <see cref="UserDetailDto"/>.
        /// </summary>
        /// <param name="user">The user domain model to convert.</param>
        /// <returns>A <see cref="UserDetailDto"/> containing the user's id, username, and role.</returns>
        public static UserDetailDto ToUserDetailDto(this User user)
        {
            return new UserDetailDto
            {
                Id = user.UserId,
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}
