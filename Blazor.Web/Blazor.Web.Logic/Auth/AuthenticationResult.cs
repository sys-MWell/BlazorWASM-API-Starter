namespace Blazor.Web.Logic.Auth
{
    /// <summary>
    /// Represents the result of an authentication operation, such as login or registration.
    /// </summary>
    public class AuthenticationResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
        public string? ErrorCode { get; set; }
    }
}