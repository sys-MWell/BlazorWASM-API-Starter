using System.ComponentModel.DataAnnotations;

namespace App.Models.Dtos
{
    public class LoginUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
    }
}
