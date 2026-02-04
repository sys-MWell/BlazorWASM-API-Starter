using System.ComponentModel.DataAnnotations;

namespace Template.Models.Dtos
{
    public class LoginUserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string UserPassword { get; set; } = string.Empty;
    }
}
