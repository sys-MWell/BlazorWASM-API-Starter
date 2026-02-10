using Template.Models.Dtos;

namespace Blazor.Web.Logic.Auth
{
    /// <summary>
    /// Defines the contract for authentication operations including login, registration, and logout.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="user">The login credentials containing username and password.</param>
        /// <returns>An <see cref="AuthenticationResult"/> indicating success or failure with error details.</returns>
        Task<AuthenticationResult> LoginAsync(LoginUserDto user);

        /// <summary>
        /// Registers a new user account with the provided registration details.
        /// </summary>
        /// <param name="user">The registration details including username, password, and other required information.</param>
        /// <returns>An <see cref="AuthenticationResult"/> indicating success or failure with error details.</returns>
        Task<AuthenticationResult> RegisterAsync(RegisterUserDto user);

        /// <summary>
        /// Logs out the current user and clears the session.
        /// </summary>
        /// <returns>A task representing the asynchronous logout operation.</returns>
        Task LogoutAsync();
    }
}
