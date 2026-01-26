namespace App.Models.Dtos
{
    public class UserLoginResponseDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}