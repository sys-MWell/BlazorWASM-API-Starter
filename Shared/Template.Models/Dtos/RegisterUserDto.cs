using System.ComponentModel.DataAnnotations;

namespace Template.Models.Dtos
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; } = default!;
        [Required]
        public string UserPassword { get; set; } = default!;
        public string? Role { get; set; }
    }
}
