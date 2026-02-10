using System;
using System.Threading.Tasks;

namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Defines a simple storage abstraction for an access token and its expiration.
    /// Provides methods to set, clear, and evaluate whether the current token is expired.
    /// </summary>
    public interface ITokenStore
    {
        /// <summary>
        /// Gets the current access token, or null if not set.
        /// </summary>
        string? AccessToken { get; }

        /// <summary>
        /// Gets the expiration time of the current token in UTC, or null if not set.
        /// </summary>
        DateTimeOffset? ExpiresUtc { get; }

        /// <summary>
        /// Stores the provided JWT token and extracts its expiration claim.
        /// </summary>
        /// <param name="token">The JWT token to store.</param>
        void SetToken(string token);

        /// <summary>
        /// Clears the stored token and expiration.
        /// </summary>
        void ClearToken();

        /// <summary>
        /// Determines whether the current token has expired.
        /// </summary>
        /// <returns>True if the token is expired or not set; otherwise false.</returns>
        bool IsExpired();
    }
}
