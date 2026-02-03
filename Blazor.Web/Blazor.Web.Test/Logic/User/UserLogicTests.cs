using Blazor.Web.Logic.User;
using Blazor.Web.Domain.Validation;
using Blazor.Web.Domain.Auth;
using Blazor.Web.Repository.User;
using Template.Models.Dtos;
using Template.Models.Models;

namespace Blazor.Web.Test.Logic.User
{
    /// <summary>
    /// Tests for <see cref="UserLogic"/> covering success, token missing, token expired and JSON error mapping.
    /// </summary>
    [TestClass]
    public class UserLogicTests
    {
        private class RepoStub : IUserRepository
        {
            private readonly ApiResponse<AuthResponseDto> _response;
            public RepoStub(ApiResponse<AuthResponseDto> response) => _response = response;
            public Task<ApiResponse<AuthResponseDto>> UserLoginAsync(LoginUserDto loginCredentials) => Task.FromResult(_response);
            public Task<ApiResponse<AuthResponseDto>> UserRegisterAsync(RegisterUserDto registerCredentials) => throw new NotImplementedException();
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

        /// <summary>
        /// Successful login should return user data and set session via token store.
        /// </summary>
        [TestMethod]
        public async Task UserLoginAsync_Success_SetsSessionAndReturnsUser()
        {
            var dto = new AuthResponseDto { Token = "t", User = new UserDetailDto { Id = 1, Username = "u" } };
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true, Data = dto, ErrorCode = AppErrorCode.None });
            var tokenStore = new InMemoryTokenStore();
            var logic = new UserLogic(repo, tokenStore, new LogicValidator(), new SessionStub());

            var result = await logic.UserLoginAsync(new LoginUserDto { Username = "u", UserPassword = "p" });
            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(1, result.Data!.Id);
        }

        /// <summary>
        /// When the token is missing/empty, logic returns a TokenMissing failure.
        /// </summary>
        [TestMethod]
        public async Task UserLoginAsync_TokenMissing_Fails()
        {
            var dto = new AuthResponseDto { Token = "", User = new UserDetailDto { Id = 1, Username = "u" } };
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true, Data = dto });
            var logic = new UserLogic(repo, new InMemoryTokenStore(), new LogicValidator(), new SessionStub());

            var result = await logic.UserLoginAsync(new LoginUserDto());
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(AppErrorCode.TokenMissing, result.ErrorCode);
        }

        /// <summary>
        /// If the token store deems the token expired, logic returns a TokenExpired failure and clears token.
        /// </summary>
        [TestMethod]
        public async Task UserLoginAsync_TokenExpired_Fails()
        {
            var dto = new AuthResponseDto { Token = "t", User = new UserDetailDto { Id = 1, Username = "u" } };
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = true, Data = dto });
            var logic = new UserLogic(repo, new ExpiringTokenStore(), new LogicValidator(), new SessionStub());

            var result = await logic.UserLoginAsync(new LoginUserDto());
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(AppErrorCode.TokenExpired, result.ErrorCode);
        }

        /// <summary>
        /// When repository returns a JSON error payload, logic maps error message and code appropriately.
        /// </summary>
        [TestMethod]
        public async Task UserLoginAsync_MapsJsonErrorMessageAndCode()
        {
            var repo = new RepoStub(new ApiResponse<AuthResponseDto> { IsSuccess = false, ErrorMessage = "{\"error\":\"Bad creds\",\"code\":3004}" });
            var logic = new UserLogic(repo, new InMemoryTokenStore(), new LogicValidator(), new SessionStub());
            var result = await logic.UserLoginAsync(new LoginUserDto());
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Bad creds", result.ErrorMessage);
            Assert.AreEqual(AppErrorCode.LoginFailed, result.ErrorCode);
        }
    }
}
