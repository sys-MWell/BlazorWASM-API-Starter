namespace Template.Models.Dtos
{
    // Represents basic authentication-related user details.
    public class AuthDetailsDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string? Email { get; set; }
    }
}
