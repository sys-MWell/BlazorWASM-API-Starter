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
        string? AccessToken { get; }
        DateTimeOffset? ExpiresUtc { get; }
        void SetToken(string token);
        void ClearToken();
        bool IsExpired();
    }
}
