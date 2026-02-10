using Template.Models.Dtos;

namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Holds the currently authenticated user's details for the lifetime of a circuit (and can be cleared on logout).
    /// </summary>
    public interface IUserSession
    {
        /// <summary>
        /// Gets the current user's details, or null if not authenticated.
        /// </summary>
        UserDetailDto? CurrentUser { get; }

        /// <summary>
        /// Sets the current user's details.
        /// </summary>
        /// <param name="user">The user details to store.</param>
        void SetUser(UserDetailDto user);

        /// <summary>
        /// Clears the current user's session data.
        /// </summary>
        void Clear();
    }
}
