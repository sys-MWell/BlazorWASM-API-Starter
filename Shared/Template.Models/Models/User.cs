using System.ComponentModel.DataAnnotations;

namespace Template.Models.Models
{
    /// <summary>
    /// Represents a user domain entity for authentication and authorization.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique user identifier.
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

        /// <summary>
        /// Gets or sets the hashed password.
        /// </summary>
        [Required]
        [StringLength(255)]
        public required string UserPassword { get; set; }

        /// <summary>
        /// Gets or sets the user's role.
        /// </summary>
        [StringLength(50)]
        public required string Role { get; set; }
    }
}
