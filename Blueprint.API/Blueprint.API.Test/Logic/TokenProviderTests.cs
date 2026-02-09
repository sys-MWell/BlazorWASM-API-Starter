using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Blueprint.API.Logic.Helpers;
using Template.Models.Dtos;

namespace Blueprint.API.Test.Logic
{
    /// <summary>
    /// Unit tests for <see cref="TokenProvider"/> JWT generation behavior.
    /// </summary>
    [TestClass]
    public sealed class TokenProviderTests
    {
        private sealed class TestLogger : ILogger<TokenProvider>
        {
            public IDisposable BeginScope<TState>(TState state) where TState : notnull => new Noop();
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
            private sealed class Noop : IDisposable { public void Dispose() { } }
        }

        private static IConfiguration BuildConfig(string? key = null, string? issuer = null, string? audience = null, string? expiresInMinutes = null)
        {
            var dict = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = key ?? new string('x', 64),
                ["Jwt:Issuer"] = issuer ?? "test-issuer",
                ["Jwt:Audience"] = audience ?? "test-audience",
                ["Jwt:ExpiresInMinutes"] = expiresInMinutes ?? "60"
            };
            return new ConfigurationBuilder().AddInMemoryCollection(dict).Build();
        }

        /// <summary>
        /// Verifies that a valid JWT token is generated with correct claims.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_ValidConfig_ReturnsTokenWithClaims()
        {
            var logger = new TestLogger();
            var config = BuildConfig();
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 42, Username = "alice", Role = "Admin" };

            var response = provider.GenerateAuthResponse(user);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Token);
            Assert.AreEqual("alice", response.User.Username);
            Assert.AreEqual("Admin", response.User.Role);

            // Decode and verify claims
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.Token);

            Assert.AreEqual("42", token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            Assert.AreEqual("alice", token.Claims.First(c => c.Type == ClaimTypes.Name).Value);
            Assert.AreEqual("Admin", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        /// <summary>
        /// Verifies that the token expiration is set correctly based on configuration.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_SetsCorrectExpiration()
        {
            var logger = new TestLogger();
            var config = BuildConfig(expiresInMinutes: "30");
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 1, Username = "bob", Role = "User" };

            var beforeGeneration = DateTime.UtcNow;
            var response = provider.GenerateAuthResponse(user);
            var afterGeneration = DateTime.UtcNow;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.Token);

            // Token should expire approximately 30 minutes from now
            var expectedExpiry = beforeGeneration.AddMinutes(30);
            var maxExpiry = afterGeneration.AddMinutes(30).AddSeconds(5); // allow small buffer

            Assert.IsTrue(token.ValidTo >= expectedExpiry.AddSeconds(-5), $"Token expires too early: {token.ValidTo}");
            Assert.IsTrue(token.ValidTo <= maxExpiry, $"Token expires too late: {token.ValidTo}");
        }

        /// <summary>
        /// Verifies that issuer and audience are set correctly in the token.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_SetsIssuerAndAudience()
        {
            var logger = new TestLogger();
            var config = BuildConfig(issuer: "my-issuer", audience: "my-audience");
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 1, Username = "test", Role = "User" };

            var response = provider.GenerateAuthResponse(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.Token);

            Assert.AreEqual("my-issuer", token.Issuer);
            Assert.IsTrue(token.Audiences.Contains("my-audience"));
        }

        /// <summary>
        /// Verifies that an exception is thrown when JWT key is missing.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_MissingKey_ThrowsException()
        {
            var logger = new TestLogger();
            var config = BuildConfig(key: "");
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 1, Username = "test", Role = "User" };

            var exceptionThrown = false;
            try
            {
                provider.GenerateAuthResponse(user);
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected InvalidOperationException was not thrown.");
        }

        /// <summary>
        /// Verifies that an exception is thrown when expiration config is invalid.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_InvalidExpiration_ThrowsException()
        {
            var logger = new TestLogger();
            var config = BuildConfig(expiresInMinutes: "not-a-number");
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 1, Username = "test", Role = "User" };

            var exceptionThrown = false;
            try
            {
                provider.GenerateAuthResponse(user);
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown, "Expected InvalidOperationException was not thrown.");
        }

        /// <summary>
        /// Verifies that null role defaults to "Default" in claims.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_NullRole_DefaultsToDefault()
        {
            var logger = new TestLogger();
            var config = BuildConfig();
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 1, Username = "test", Role = null };

            var response = provider.GenerateAuthResponse(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.Token);

            Assert.AreEqual("Default", token.Claims.First(c => c.Type == ClaimTypes.Role).Value);
        }

        /// <summary>
        /// Verifies that an expired token can be detected by comparing ValidTo to current time.
        /// This simulates testing token expiration logic.
        /// </summary>
        [TestMethod]
        public void GenerateAuthResponse_TokenValidTo_CanBeUsedForExpirationCheck()
        {
            var logger = new TestLogger();
            var config = BuildConfig(expiresInMinutes: "1"); // 1 minute expiry
            var provider = new TokenProvider(logger, config);
            var user = new UserDetailDto { Id = 1, Username = "test", Role = "User" };

            var response = provider.GenerateAuthResponse(user);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.Token);

            // Token should NOT be expired immediately after creation
            Assert.IsTrue(token.ValidTo > DateTime.UtcNow, "Token should not be expired immediately after creation");

            // Simulating expired check: if ValidTo < UtcNow, token is expired
            // This is how ASP.NET Core JWT middleware validates lifetime
            var futureTime = DateTime.UtcNow.AddMinutes(5);
            Assert.IsTrue(token.ValidTo < futureTime, "Token should expire before 5 minutes from now (configured for 1 min)");
        }
    }
}
