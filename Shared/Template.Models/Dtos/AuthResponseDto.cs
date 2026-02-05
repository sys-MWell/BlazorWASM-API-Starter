namespace Template.Models.Dtos
{
    /// <summary>
    /// Standard authentication response containing a JWT token and user details.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Gets or sets the JWT token.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the authenticated user details.
        /// </summary>
        public UserDetailDto User { get; set; } = new UserDetailDto
        {
            Id = 0,
            Username = string.Empty,
            Role = null
        };

    }
}