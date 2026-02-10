using Blazor.Web.Domain.Auth;
using Blazor.Web.Test.Utils;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Claims;

namespace Blazor.Web.Test.Domain.Auth
{
    /// <summary>
    /// Unit tests for <see cref="CustomAuthenticationStateProvider"/> covering anonymous, login, logout and persistence flows.
    /// </summary>
    [TestClass]
    public class CustomAuthenticationStateProviderTests
    {
        /// <summary>
        /// When no token is present, authentication state should be anonymous.
        /// </summary>
        [TestMethod]
        public async Task GetAuthenticationStateAsync_NoToken_ReturnsAnonymous()
        {
            var provider = new CustomAuthenticationStateProvider(new InMemoryTokenStore(), new TestTokenPersistence(), new TestUserSession(), NullLogger<CustomAuthenticationStateProvider>.Instance);
            var state = await provider.GetAuthenticationStateAsync();
            Assert.AreEqual(false, state.User.Identity?.IsAuthenticated);
        }

        /// <summary>
        /// Valid token should mark the principal as authenticated and set the session user.
        /// </summary>
        [TestMethod]
        public async Task MarkUserAsAuthenticatedAsync_SetsPrincipal_And_Session()
        {
            var tokenStore = new InMemoryTokenStore();
            var persistence = new TestTokenPersistence();
            var session = new TestUserSession();
            var provider = new CustomAuthenticationStateProvider(tokenStore, persistence, session, NullLogger<CustomAuthenticationStateProvider>.Instance);

            var jwt = Blazor.Web.Test.Utils.JwtTestHelper.CreateJwt(7, username: "bob", role: "Admin");
            await provider.MarkUserAsAuthenticatedAsync(jwt);

            var state = await provider.GetAuthenticationStateAsync();
            Assert.AreEqual(true, state.User.Identity?.IsAuthenticated);
            Assert.AreEqual("7", state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.AreEqual("bob", state.User.FindFirst(ClaimTypes.Name)?.Value);
            Assert.AreEqual("Admin", state.User.FindFirst(ClaimTypes.Role)?.Value);
            Assert.IsNotNull(session.CurrentUser);
            Assert.AreEqual(7, session.CurrentUser!.Id);
        }

        /// <summary>
        /// Logout should clear token, session and set anonymous principal.
        /// </summary>
        [TestMethod]
        public async Task MarkUserAsLoggedOutAsync_ClearsState()
        {
            var tokenStore = new InMemoryTokenStore();
            var persistence = new TestTokenPersistence();
            var session = new TestUserSession();
            var provider = new CustomAuthenticationStateProvider(tokenStore, persistence, session, NullLogger<CustomAuthenticationStateProvider>.Instance);

            await provider.MarkUserAsLoggedOutAsync();
            var state = await provider.GetAuthenticationStateAsync();
            Assert.AreEqual(false, state.User.Identity?.IsAuthenticated);
            Assert.IsNull(session.CurrentUser);
        }

        /// <summary>
        /// The provider should restore from persistence only once and authenticate accordingly.
        /// </summary>
        [TestMethod]
        public async Task RestoreFromPersistenceAsync_OneTime_RestoresToken()
        {
            var tokenStore = new InMemoryTokenStore();
            var persistence = new TestTokenPersistence();
            var session = new TestUserSession();
            var provider = new CustomAuthenticationStateProvider(tokenStore, persistence, session, NullLogger<CustomAuthenticationStateProvider>.Instance);

            var jwt = Blazor.Web.Test.Utils.JwtTestHelper.CreateJwt(11, username: "eve");
            await persistence.SaveTokenAsync(jwt);

            await provider.RestoreFromPersistenceAsync();
            var state = await provider.GetAuthenticationStateAsync();
            Assert.AreEqual(true, state.User.Identity?.IsAuthenticated);
            Assert.AreEqual("11", state.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    }
}
