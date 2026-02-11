using Blazor.Web.Domain.Auth;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace Blazor.Web.Auth
{
    /// <summary>
    /// Token persistence implementation using ProtectedLocalStorage to persist JWT tokens
    /// across browser refreshes. Uses encryption to protect the token in browser storage.
    /// </summary>
    public class ProtectedSessionTokenPersistence : ITokenPersistence
    {
        private const string TokenKey = "auth_token";
        private readonly ProtectedLocalStorage _localStorage;
        private readonly ILogger<ProtectedSessionTokenPersistence> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtectedSessionTokenPersistence"/> class.
        /// </summary>
        /// <param name="localStorage">The protected local storage service.</param>
        /// <param name="logger">The logger instance.</param>
        public ProtectedSessionTokenPersistence(ProtectedLocalStorage localStorage, ILogger<ProtectedSessionTokenPersistence> logger)
        {
            _localStorage = localStorage ?? throw new ArgumentNullException(nameof(localStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task SaveTokenAsync(string token)
        {
            try
            {
                await _localStorage.SetAsync(TokenKey, token);
                _logger.LogDebug("Token persisted to local storage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to persist token to local storage");
            }
        }

        /// <inheritdoc />
        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var result = await _localStorage.GetAsync<string>(TokenKey);
                if (result.Success && !string.IsNullOrWhiteSpace(result.Value))
                {
                    _logger.LogDebug("Token retrieved from local storage");
                    return result.Value;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve token from local storage");
            }
            return null;
        }

        /// <inheritdoc />
        public async Task ClearTokenAsync()
        {
            try
            {
                await _localStorage.DeleteAsync(TokenKey);
                _logger.LogDebug("Token cleared from local storage");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clear token from local storage");
            }
        }
    }
}
