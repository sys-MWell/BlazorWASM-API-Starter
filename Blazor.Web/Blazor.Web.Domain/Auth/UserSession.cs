using Template.Models.Dtos;

namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Provides an in-memory implementation of <see cref="IUserSession"/> for storing 
    /// the currently authenticated user's details during a session.
    /// </summary>
    public class UserSession : IUserSession
    {
        /// <inheritdoc />
        public UserDetailDto? CurrentUser { get; private set; }

        /// <inheritdoc />
        public void SetUser(UserDetailDto user) => CurrentUser = user;

        /// <inheritdoc />
        public void Clear() => CurrentUser = null;
    }
}
