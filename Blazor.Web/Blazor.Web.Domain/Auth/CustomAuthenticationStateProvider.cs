using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using App.Models.Dtos;

namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Authentication state provider backed by a JWT whose NameIdentifier is the User Id.
    /// Identity.Name resolves to the User Id (not the username)
    /// </summary>
    public class CustomAuthenticationStateProvider(ITokenStore tokenStore, ITokenPersistence persistence, IUserSession userSession) : AuthenticationStateProvider
    {
        private readonly ITokenStore _tokenStore = tokenStore;
        private readonly ITokenPersistence _persistence = persistence;
        private readonly IUserSession _userSession = userSession;
        private ClaimsPrincipal _cachedPrincipal = new(new ClaimsIdentity());
        private bool _restored;

        /// <summary>
        /// Gets the currently resolved user details from the JWT (Id, Username, Role) or null if unauthenticated.
        /// </summary>
        public UserDetailDto? CurrentUser { get; private set; }

        /// <summary>
        /// Resolves the current <see cref="AuthenticationState"/> based on the stored JWT.
        /// Returns an empty (unauthenticated) principal if no token exists or it is expired.
        /// Caches the principal to avoid re-parsing the JWT on subsequent calls.
        /// </summary>
        /// <returns>
        /// A task producing the current <see cref="AuthenticationState"/> representing authenticated or anonymous user.
        /// </returns>
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (string.IsNullOrWhiteSpace(_tokenStore.AccessToken) || _tokenStore.IsExpired())
            {
                _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
                CurrentUser = null;
                _userSession.Clear();
                return Task.FromResult(new AuthenticationState(_cachedPrincipal));
            }

            if (_cachedPrincipal.Identity?.IsAuthenticated == true)
            {
                if (CurrentUser is not null && _userSession.CurrentUser is null)
                {
                    _userSession.SetUser(CurrentUser);
                }
                return Task.FromResult(new AuthenticationState(_cachedPrincipal));
            }

            _cachedPrincipal = BuildPrincipal(_tokenStore.AccessToken);
            return Task.FromResult(new AuthenticationState(_cachedPrincipal));
        }

        /// <summary>
        /// Attempts a one-time restore of a persisted JWT token and updates the authentication state if successful.
        /// Subsequent calls are ignored after the first restore attempt.
        /// </summary>
        /// <returns>A task representing the asynchronous restore operation.</returns>
        public async Task RestoreFromPersistenceAsync()
        {
            if (_restored) return;
            _restored = true;
            var persisted = await _persistence.GetTokenAsync();
            if (!string.IsNullOrWhiteSpace(persisted))
            {
                await _persistence.SaveTokenAsync(persisted);
                _cachedPrincipal = BuildPrincipal(persisted);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedPrincipal)));
            }
        }

        /// <summary>
        /// Marks a user as authenticated by storing, persisting, and parsing the provided JWT token.
        /// Notifies consumers of the updated authentication state.
        /// </summary>
        /// <param name="token">A valid JWT access token.</param>
        /// <returns>A task representing the asynchronous authenticate operation.</returns>
        public async Task MarkUserAsAuthenticatedAsync(string token)
        {

            await _persistence.SaveTokenAsync(token);
            _cachedPrincipal = BuildPrincipal(token);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedPrincipal)));
        }

        /// <summary>
        /// Logs the user out by clearing the stored and persisted token and resetting the principal and session.
        /// Notifies consumers of the updated (anonymous) authentication state.
        /// </summary>
        /// <returns>A task representing the asynchronous logout operation.</returns>
        public async Task MarkUserAsLoggedOutAsync()
        {
            await _persistence.ClearTokenAsync();
            _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            CurrentUser = null;
            _userSession.Clear();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_cachedPrincipal)));
        }

        /// <summary>
        /// Builds a <see cref="ClaimsPrincipal"/> from the provided JWT token and populates <see cref="CurrentUser"/> and session.
        /// Returns an empty principal if the token cannot be parsed.
        /// </summary>
        /// <param name="token">The JWT token string.</param>
        /// <returns>A claims principal representing the user described by the token or anonymous if invalid.</returns>
        private ClaimsPrincipal BuildPrincipal(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwt = handler.ReadJwtToken(token);
                var identity = new ClaimsIdentity(jwt.Claims, "jwt", ClaimTypes.NameIdentifier, ClaimTypes.Role);
                var principal = new ClaimsPrincipal(identity);
                var idString = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                if (!int.TryParse(idString, out var id))
                {
                    id = 0;
                }

                var username = principal.FindFirst(ClaimTypes.Name)?.Value
                               ?? principal.FindFirst("username")?.Value
                               ?? principal.FindFirst("unique_name")?.Value
                               ?? id.ToString();
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                CurrentUser = new UserDetailDto { Id = id, Username = username, Role = role };
                _userSession.SetUser(CurrentUser);
                return principal;
            }
            catch
            {
                CurrentUser = null;
                _userSession.Clear();
                return new ClaimsPrincipal(new ClaimsIdentity());
            }
        }
    }
}
