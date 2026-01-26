using System.ComponentModel.DataAnnotations;

namespace App.Models.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Username { get; set; }

        [Required]
        [StringLength(255)]
        public required string UserPassword { get; set; }

        [StringLength(50)]
        public required string Role { get; set; }
    }
}
