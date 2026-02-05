using System.ComponentModel.DataAnnotations;

namespace Template.Models.Dtos
{
    /// <summary>
    /// Request payload for user login containing username and password.
    /// </summary>
    public class LoginUserDto
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the plaintext password.
        /// </summary>
        [Required]
        public string UserPassword { get; set; } = string.Empty;
    }
}
