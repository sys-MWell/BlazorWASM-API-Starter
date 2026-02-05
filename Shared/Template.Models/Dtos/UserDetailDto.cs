using System.ComponentModel.DataAnnotations;

namespace Template.Models.Dtos
{
    /// <summary>
    /// Represents a user identity returned by authentication flows.
    /// </summary>
    public class UserDetailDto
    {
        /// <summary>
        /// Gets or sets the unique user identifier.
        /// </summary>
        [Required]
        public required int Id { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; } = default!;
        /// <summary>
        /// Gets or sets the role of the user.
        /// </summary>
        public string? Role { get; set; }
    }
}