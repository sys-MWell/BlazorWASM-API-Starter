namespace Template.Models.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public UserDetailDto User { get; set; } = new UserDetailDto
        {
            Id = 0,
            Username = string.Empty,
            Role = null
        };

    }
}