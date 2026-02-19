using Blazor.Web.Logic.Auth;
using Blazor.Web.Logic.Services.Validation;
using Blazor.Web.Domain.Validation;
using Blazor.Web.Domain.Auth;
using Blazor.Web.Repository.User;
using Blazor.Web.Models.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Template.Models.Dtos;
using Template.Models.Models;

namespace Blazor.Web.Test.Logic.Auth
{
    /// <summary>
    /// Tests for <see cref="AuthenticationService"/> covering success, token missing, token expired and JSON error mapping.
    /// </summary>
    [TestClass]
    public class AuthenticationServiceTests
    {
        private class RepoStub : IUserRepository
        {
            private readonly ApiResponse<AuthResponseDto> _response;
            public RepoStub(ApiResponse<AuthResponseDto> response) => _response = response;
            public Task<ApiResponse<AuthResponseDto>> UserLoginAsync(LoginUserDto loginCredentials) => Task.FromResult(_response);
            public Task<ApiResponse<AuthResponseDto>> UserRegisterAsync(RegisterUserDto registerCredentials) => Task.FromResult(_response);
        }

        private sealed class ExpiringTokenStore : ITokenStore
        {
            public string? AccessToken { get; private set; }
            public DateTimeOffset? ExpiresUtc { get; private set; }
            public void SetToken(string token) { AccessToken = token; ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(-1); }
            public void ClearToken() { AccessToken = null; ExpiresUtc = null; }
            public bool IsExpired() => true;
        }

        private class SessionStub : IUserSession
        {
            public UserDetailDto? CurrentUser { get; private set; }
            public void SetUser(UserDetailDto user) => CurrentUser = user;
            public void Clear() => CurrentUser = null;
        }

        private class ValidatorStub : ICredentialValidator
        {
            private readonly bool _isValid;
            public ValidatorStub(bool isValid = true) => _isValid = isValid;
            public CredentialValidationResult ValidateCredentials(string? username, string? password, bool forLogin)
            {
                var result = new CredentialValidationResult();
                if (!_isValid)
                {
                    result.Errors.Add(new ValidationError("Username", "Invalid", "Invalid credentials"));
                }
                return result;
            }
        }

        private static AuthenticationService CreateService(
            IUserRepository repo,
            ITokenStore? tokenStore = null,
            ICredentialValidator? credentialValidator = null,
            IUserSession? session = null)
        {
            return new AuthenticationService(
                repo,
                credentialValidator ?? new ValidatorStub(),
                tokenStore ?? new InMemoryTokenStore(),
                new LogicValidator(),
                session ?? new SessionStub(),
                NullLogger<AuthenticationService>.Instance);
        }

        /// <summary>
        /// Successful login should return success and set session via token store.
        /// </summary>
        [TestMethod]
        public async Task LoginAsync_Success_ReturnsSuccessAndSetsSession()
        {
            var dto = new AuthResponseDto { Token = "t", User = new UserDetailDto { Id = 1, Username = "u" } };
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true, Data = dto, ErrorCode = AppErrorCode.None });
            var session = new SessionStub();
            var service = CreateService(repo, session: session);

            var result = await service.LoginAsync(new LoginUserDto { Username = "u", UserPassword = "p" });

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(session.CurrentUser);
            Assert.AreEqual(1, session.CurrentUser.Id);
        }

        /// <summary>
        /// When the token is missing/empty, service returns a TokenMissing failure.
        /// </summary>
        [TestMethod]
        public async Task LoginAsync_TokenMissing_Fails()
        {
            var dto = new AuthResponseDto { Token = "", User = new UserDetailDto { Id = 1, Username = "u" } };
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true, Data = dto });
            var service = CreateService(repo);

            var result = await service.LoginAsync(new LoginUserDto { Username = "u", UserPassword = "p" });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(AppErrorCode.TokenMissing.ToString(), result.ErrorCode);
        }

        /// <summary>
        /// If the token store deems the token expired, service returns a TokenExpired failure and clears token.
        /// </summary>
        [TestMethod]
        public async Task LoginAsync_TokenExpired_Fails()
        {
            var dto = new AuthResponseDto { Token = "t", User = new UserDetailDto { Id = 1, Username = "u" } };
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true, Data = dto });
            var service = CreateService(repo, tokenStore: new ExpiringTokenStore());

            var result = await service.LoginAsync(new LoginUserDto { Username = "u", UserPassword = "p" });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(AppErrorCode.TokenExpired.ToString(), result.ErrorCode);
        }

        /// <summary>
        /// When repository returns a JSON error payload, service maps error message and code appropriately.
        /// </summary>
        [TestMethod]
        public async Task LoginAsync_MapsJsonErrorMessageAndCode()
        {
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = false, ErrorMessage = "{\"error\":\"Bad creds\",\"code\":3004}" });
            var service = CreateService(repo);

            var result = await service.LoginAsync(new LoginUserDto { Username = "u", UserPassword = "p" });

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Contains("Bad creds"));
            Assert.AreEqual(AppErrorCode.LoginFailed.ToString(), result.ErrorCode);
        }

        /// <summary>
        /// Credential validation failure should return errors without calling repository.
        /// </summary>
        [TestMethod]
        public async Task LoginAsync_ValidationFails_ReturnsErrors()
        {
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true });
            var service = CreateService(repo, credentialValidator: new ValidatorStub(isValid: false));

            var result = await service.LoginAsync(new LoginUserDto { Username = "", UserPassword = "" });

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.Errors.Count > 0);
        }

        /// <summary>
        /// Logout should clear token and session.
        /// </summary>
        [TestMethod]
        public async Task LogoutAsync_ClearsTokenAndSession()
        {
            var tokenStore = new InMemoryTokenStore();
            tokenStore.SetToken("test-token");
            var session = new SessionStub();
            session.SetUser(new UserDetailDto { Id = 1, Username = "u" });

            var repo = new RepoStub(new ApiResponse<AuthResponseDto>());
            var service = CreateService(repo, tokenStore: tokenStore, session: session);

            await service.LogoutAsync();

            Assert.IsNull(tokenStore.AccessToken);
            Assert.IsNull(session.CurrentUser);
        }
    }
}
