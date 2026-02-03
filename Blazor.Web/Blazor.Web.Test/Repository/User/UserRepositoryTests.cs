using Blazor.Web.Repository.User;
using Blazor.Web.Domain.Shared;
using Microsoft.Extensions.Logging.Abstractions;
using App.Web.Test.Utils;
using Template.Models.Dtos;
using Template.Models.Models;
using System.Net;
using System.Net.Http.Json;

namespace App.Web.Test.Repository.User
{
    /// <summary>
    /// Tests for <see cref="UserRepository"/> covering success and failure HTTP scenarios.
    /// </summary>
    [TestClass]
    public class UserRepositoryTests
    {
        private static IHttpClientFactory MakeFactory(HttpResponseMessage message)
        {
            return HttpClientFactoryStub.FromHandler(_ => message);
        }

        /// <summary>
        /// A successful response should deserialize into <see cref="AuthResponseDto"/>.
        /// </summary>
        [TestMethod]
        public async Task UserLoginAsync_Success_ReturnsAuthResponse()
        {
            var payload = new AuthResponseDto { Token = "tok", User = new UserDetailDto { Id = 2, Username = "john" } };
            var msg = new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(payload) };
            var repo = new UserRepository(MakeFactory(msg), NullLogger<UserRepository>.Instance, new ApiSettings { AppApiBaseUrl = "http://x/" });
            var res = await repo.UserLoginAsync(new LoginUserDto());
            Assert.IsTrue(res.IsSuccess);
            Assert.IsNotNull(res.Data);
            Assert.AreEqual(2, res.Data!.User.Id);
            Assert.AreEqual("tok", res.Data.Token);
        }

        /// <summary>
        /// A failing response should map HTTP status code into <see cref="AppErrorCode"/> and return content as a message.
        /// </summary>
        [TestMethod]
        public async Task UserLoginAsync_Failure_MapsStatusAndContent()
        {
            var msg = new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Bad") };
            var repo = new UserRepository(MakeFactory(msg), NullLogger<UserRepository>.Instance, new ApiSettings { AppApiBaseUrl = "http://x/" });
            var res = await repo.UserLoginAsync(new LoginUserDto());
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, res.ErrorCode);
            Assert.AreEqual("Bad", res.ErrorMessage);
        }
    }
}
