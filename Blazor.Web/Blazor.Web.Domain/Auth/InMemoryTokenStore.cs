using System;
using System.IdentityModel.Tokens.Jwt;

namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// In-memory implementation of <see cref="ITokenStore"/> holding a single access token
    /// and its (optional) UTC expiration time parsed from the JWT's <c>exp</c> claim.
    /// </summary>
    public class InMemoryTokenStore : ITokenStore
    {
        /// <summary>
        /// Gets the currently stored access token (JWT) or <c>null</c> if none set.
        /// </summary>
        public string? AccessToken { get; private set; }

        /// <summary>
        /// Gets the UTC expiration timestamp of the token if it could be determined;
        /// otherwise <c>null</c> when the token has no valid expiration or failed to parse.
        /// </summary>
        public DateTimeOffset? ExpiresUtc { get; private set; }

        /// <summary>
        /// Stores the provided JWT access token and attempts to extract its expiration.
        /// </summary>
        /// <param name="token">The raw JWT access token string.</param>
        /// <remarks>
        /// If the token cannot be parsed or has no valid expiration, <see cref="ExpiresUtc"/> is set to <c>null</c>.
        /// </remarks>
        public void SetToken(string token)
        {
            AccessToken = token;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var exp = jwt.ValidTo; // UTC
                ExpiresUtc = exp == DateTime.MinValue ? null : new DateTimeOffset(exp, TimeSpan.Zero);
            }
            catch
            {
                ExpiresUtc = null;
            }
        }

        /// <summary>
        /// Clears the stored token and any associated expiration metadata.
        /// </summary>
        public void ClearToken()
        {
            AccessToken = null;
            ExpiresUtc = null;
        }

        /// <summary>
        /// Determines whether the stored token is expired.
        /// </summary>
        /// <returns>
        /// <c>true</c> if no token is stored, or the current UTC time is greater than or equal to
        /// <see cref="ExpiresUtc"/> when it has a value; otherwise <c>false</c>. If expiration is unknown
        /// (<see cref="ExpiresUtc"/> is <c>null</c>) the token is treated as not expired.
        /// </returns>
        public bool IsExpired()
        {
            if (AccessToken == null) return true;
            if (ExpiresUtc == null) return false; // unknown -> treat as not expired
            return DateTimeOffset.UtcNow >= ExpiresUtc.Value;
        }
    }
}
