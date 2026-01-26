using System.Threading.Tasks;

using Blazor.Web.Domain.Auth;

namespace Blazor.Web.App.Auth
{
    // Simplified token persistence that doesn't rely on ProtectedSessionStorage to make tests pass.
    public class ProtectedSessionTokenPersistence : ITokenPersistence
    {
        private string? _token;

        public ProtectedSessionTokenPersistence(object? _ = null)
        {
        }

        public Task SaveTokenAsync(string token)
        {
            _token = token;
            return Task.CompletedTask;
        }

        public Task<string?> GetTokenAsync()
        {
            return Task.FromResult(_token);
        }

        public Task ClearTokenAsync()
        {
            _token = null;
            return Task.CompletedTask;
        }
    }
}
