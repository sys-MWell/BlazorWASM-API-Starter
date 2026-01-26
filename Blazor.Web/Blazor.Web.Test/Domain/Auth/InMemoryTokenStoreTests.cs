using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blazor.Web.Domain.Auth;
using Blazor.Web.Test.Utils;
using System;

namespace Blazor.Web.Test.Domain.Auth
{
    /// <summary>
    /// Unit tests for <see cref="InMemoryTokenStore"/> covering token parsing, expiration, and clearing behavior.
    /// </summary>
    [TestClass]
    public class InMemoryTokenStoreTests
    {
        /// <summary>
        /// Verifies that without any token set, the store reports the token as expired.
        /// </summary>
        [TestMethod]
        public void IsExpired_NoToken_ReturnsTrue()
        {
            var store = new InMemoryTokenStore();
            Assert.IsTrue(store.IsExpired());
        }

        /// <summary>
        /// Ensures a valid JWT sets <see cref="InMemoryTokenStore.ExpiresUtc"/> and is not initially expired.
        /// </summary>
        [TestMethod]
        public void SetToken_ParsesExpiry_NotExpiredInitially()
        {
            var jwt = Blazor.Web.Test.Utils.JwtTestHelper.CreateJwt(1, expires: DateTimeOffset.UtcNow.AddMinutes(2));
            var store = new InMemoryTokenStore();
            store.SetToken(jwt);
            Assert.AreEqual(jwt, store.AccessToken);
            Assert.IsNotNull(store.ExpiresUtc);
            Assert.IsFalse(store.IsExpired());
        }

        /// <summary>
        /// When an invalid token is set, expiration remains unknown and the token is treated as not expired.
        /// </summary>
        [TestMethod]
        public void SetToken_InvalidToken_ExpiresNull_NotExpired()
        {
            var store = new InMemoryTokenStore();
            store.SetToken("not-a-jwt");
            Assert.IsNull(store.ExpiresUtc);
            Assert.IsFalse(store.IsExpired());
        }

        /// <summary>
        /// Clearing the token resets both the token and expiration and marks it as expired.
        /// </summary>
        [TestMethod]
        public void ClearToken_ClearsState()
        {
            var store = new InMemoryTokenStore();
            store.SetToken(Blazor.Web.Test.Utils.JwtTestHelper.CreateJwt(2));
            store.ClearToken();
            Assert.IsNull(store.AccessToken);
            Assert.IsNull(store.ExpiresUtc);
            Assert.IsTrue(store.IsExpired());
        }
    }
}
