namespace Blazor.Web.Logic.Auth
{
    /// <summary>
    /// Represents the result of an authentication operation, such as login or registration.
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the authentication operation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the collection of error messages encountered during authentication.
        /// </summary>
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// Gets or sets the application-specific error code, if any.
        /// </summary>
        public string? ErrorCode { get; set; }
    }
}