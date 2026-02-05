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
        public static User ToDomainUser(this RegisterUserDto dto, string passwordHash)
        {
            return new User
            {
                Username = dto.Username,
                Role = dto.Role ?? "User",
                UserPassword = passwordHash
            };
        }

        public static RegisterUserDto ToRegisterDto(this User user)
        {
            return new RegisterUserDto
            {
                Username = user.Username,
                Role = user.Role,
                UserPassword = user.UserPassword
            };
        }

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
