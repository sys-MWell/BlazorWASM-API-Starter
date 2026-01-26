using Blazor.Web.Domain.Auth;
using System.Threading.Tasks;
using App.Models.Dtos;

namespace Blazor.Web.Test.Utils
{
    public sealed class TestTokenStore : ITokenStore
    {
        public string? AccessToken { get; private set; }
        public DateTimeOffset? ExpiresUtc { get; private set; }
        public void SetToken(string token)
        {
            AccessToken = token;
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5);
        }
        public void ClearToken()
        {
            AccessToken = null;
            ExpiresUtc = null;
        }
        public bool IsExpired() => AccessToken == null || (ExpiresUtc.HasValue && DateTimeOffset.UtcNow >= ExpiresUtc.Value);
    }

    public sealed class TestTokenPersistence : ITokenPersistence
    {
        private string? _token;
        public Task SaveTokenAsync(string token) { _token = token; return Task.CompletedTask; }
        public Task<string?> GetTokenAsync() => Task.FromResult(_token);
        public Task ClearTokenAsync() { _token = null; return Task.CompletedTask; }
    }

    public sealed class TestUserSession : IUserSession
    {
        public UserDetailDto? CurrentUser { get; private set; }
        public void SetUser(UserDetailDto user) => CurrentUser = user;
        public void Clear() => CurrentUser = null;
    }
}
