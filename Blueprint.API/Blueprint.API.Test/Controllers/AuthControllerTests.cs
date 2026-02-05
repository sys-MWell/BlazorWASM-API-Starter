using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Blueprint.API.Logic.UserLogic;
using Template.Models.Dtos;
using Template.Models.Models;
using Blueprint.API.Controllers; // Add this using directive at the top of the file
using Blueprint.API.Logic.Helpers;

namespace Blueprint.API.Test.Controllers
{
    /// <summary>
    /// Contains unit tests for the <see cref="Blueprint.API.Controllers.AuthController"/> endpoints.
    /// </summary>
    [TestClass]
    public sealed class AuthControllerTests
    {
        private sealed class TestLogger<T> : ILogger<T>
        {
            public IDisposable BeginScope<TState>(TState state) => new Noop();
            public bool IsEnabled(LogLevel logLevel) => true;
            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, System.Exception? exception, System.Func<TState, System.Exception?, string> formatter) { }
            private sealed class Noop : System.IDisposable { public void Dispose() { } }
        }

        private sealed class StubUserLogic : IAuthLogic
        {
            public ApiResponse<UserDetailDto>? RegisterUserResponse { get; set; }
            public ApiResponse<UserDetailDto>? LoginUserResponse { get; set; }
            public ApiResponse<UserDetailDto>? GetByUsernameResponse { get; set; }

            public Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username)
                => Task.FromResult(GetByUsernameResponse ?? new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = username } });

            public Task<ApiResponse<UserDetailDto>> LoginUser(LoginUserDto userLogin)
                => Task.FromResult(LoginUserResponse ?? new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = userLogin.Username } });

            public Task<ApiResponse<UserDetailDto>> RegisterUser(RegisterUserDto userRegister)
                => Task.FromResult(RegisterUserResponse ?? new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = userRegister.Username} });
        }

        private sealed class StubTokenProvider : ITokenProvider
        {
            public AuthResponseDto GenerateAuthResponse(UserDetailDto user)
            {
                return new AuthResponseDto { Token = "stub-token", User = user };
            }
        }

        private IConfiguration BuildConfig()
        {
            var dict = new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "test-issuer",
                ["Jwt:Audience"] = "test-audience",
                ["Jwt:Key"] = new string('x', 64),
                ["Jwt:ExpiresInMinutes"] = "60"
            };
            return new ConfigurationBuilder().AddInMemoryCollection(dict!).Build();
        }

        [TestMethod]
        public async Task RegisterUser_ReturnsOk_WithTokenAndUser()
        {
            var logger = new TestLogger<AuthController>();
            var logic = new StubUserLogic
            {
                RegisterUserResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = new UserDetailDto { Id = 123, Username = "bob", Role = "Admin" },
                    ErrorCode = AppErrorCode.None
                }
            };
            var controller = new AuthController(logger, logic, new StubTokenProvider());

            var result = await controller.RegisterUser(new RegisterUserDto { Username = "bob", UserPassword = "password123" });

            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            var payload = ok!.Value! as AuthResponseDto;
            Assert.IsNotNull(payload);
            Assert.AreEqual("bob", payload!.User.Username);
            Assert.AreEqual("Admin", payload!.User.Role);
            Assert.AreEqual("stub-token", payload!.Token);
        }

        [TestMethod]
        public async Task RegisterUser_MapsError_FromLogic()
        {
            var logger = new TestLogger<AuthController>();
            var logic = new StubUserLogic
            {
                RegisterUserResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "User already exists",
                    ErrorCode = AppErrorCode.UserAlreadyExists
                }
            };
            var controller = new AuthController(logger, logic, new StubTokenProvider());

            var result = await controller.RegisterUser(new RegisterUserDto { Username = "bob", UserPassword = "password123" });

            var conflict = result.Result as ObjectResult;
            Assert.IsNotNull(conflict);
            Assert.AreEqual(409, conflict!.StatusCode);
        }

        [TestMethod]
        public async Task Login_ReturnsOk_WithTokenAndUser()
        {
            var logger = new TestLogger<AuthController>();
            var logic = new StubUserLogic
            {
                LoginUserResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = new UserDetailDto { Id = 1, Username = "alice", Role = "User" },
                    ErrorCode = AppErrorCode.None
                }
            };
            var controller = new AuthController(logger, logic, new StubTokenProvider());

            var result = await controller.Login(new LoginUserDto { Username = "alice", UserPassword = "p@ss" });

            var ok = result.Result as OkObjectResult;
            Assert.IsNotNull(ok);
            var payload = ok!.Value! as AuthResponseDto;
            Assert.IsNotNull(payload);
            Assert.AreEqual("alice", payload!.User.Username);
            Assert.AreEqual("stub-token", payload!.Token);
        }

        [TestMethod]
        public async Task Login_MapsError_FromLogic()
        {
            var logger = new TestLogger<AuthController>();
            var logic = new StubUserLogic
            {
                LoginUserResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid password",
                    ErrorCode = AppErrorCode.PasswordInvalid
                }
            };
            var controller = new AuthController(logger, logic, new StubTokenProvider());

            var result = await controller.Login(new LoginUserDto { Username = "eve", UserPassword = "bad" });

            var unauthorized = result.Result as ObjectResult;
            Assert.IsNotNull(unauthorized);
            Assert.AreEqual(401, unauthorized!.StatusCode);
        }
    }
}
