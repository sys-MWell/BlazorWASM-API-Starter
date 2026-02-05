using System.ComponentModel.DataAnnotations;

namespace Template.Models.Dtos
{
    /// <summary>
    /// Request payload for user registration containing username, password, and optional role.
    /// </summary>
    public class RegisterUserDto
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; } = default!;
        /// <summary>
        /// Gets or sets the plaintext password.
        /// </summary>
        [Required]
        public string UserPassword { get; set; } = default!;
    }
}
