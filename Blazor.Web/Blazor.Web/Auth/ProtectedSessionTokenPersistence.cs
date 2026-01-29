using Blazor.Web.Domain.Auth;

namespace Blazor.Web.Auth
{
    /// <summary>
    /// Token persistence adapter that delegates to an <see cref="ITokenStore"/> for actual storage.
    /// Provides async interface over the underlying synchronous token store to avoid duplicating token storage logic.
    /// </summary>
    public class ProtectedSessionTokenPersistence : ITokenPersistence
    {
        private readonly ITokenStore _tokenStore;

        public ProtectedSessionTokenPersistence(ITokenStore tokenStore)
        {
            _tokenStore = tokenStore ?? throw new ArgumentNullException(nameof(tokenStore));
        }

        public Task SaveTokenAsync(string token)
        {
            _tokenStore.SetToken(token);
            return Task.CompletedTask;
        }

        public Task<string?> GetTokenAsync()
        {
            return Task.FromResult(_tokenStore.AccessToken);
        }

        public Task ClearTokenAsync()
        {
            _tokenStore.ClearToken();
            return Task.CompletedTask;
        }
    }
}
