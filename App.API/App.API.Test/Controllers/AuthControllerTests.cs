using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using App.API.Controllers;
using App.API.Logic.UserLogic;
using App.Models.Dtos;
using App.Models.Models;

namespace App.API.Test.Controllers
{
    /// <summary>
    /// Unit tests for the <see cref="App.API.Controllers.AuthController"/> endpoints.
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

        private sealed class StubUserLogic : IUserLogic
        {
            public ApiResponse<UserDetailDto>? RegisterUserResponse { get; set; }
            public ApiResponse<IEnumerable<UserDetailDto>>? LoginUserResponse { get; set; }
            public ApiResponse<IEnumerable<UserDetailDto>>? GetByUsernameResponse { get; set; }

            public Task<ApiResponse<IEnumerable<UserDetailDto>>> GetUserByUsername(string username)
                => Task.FromResult(GetByUsernameResponse ?? new ApiResponse<IEnumerable<UserDetailDto>> { IsSuccess = true, Data = Enumerable.Empty<UserDetailDto>() });

            public Task<ApiResponse<IEnumerable<UserDetailDto>>> LoginUser(LoginUserDto userLogin)
                => Task.FromResult(LoginUserResponse ?? new ApiResponse<IEnumerable<UserDetailDto>> { IsSuccess = true, Data = Enumerable.Empty<UserDetailDto>() });

            public Task<ApiResponse<UserDetailDto>> RegisterUser(RegisterUserDto userRegister)
                => Task.FromResult(RegisterUserResponse ?? new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = userRegister.Username, Role = userRegister.Role } });
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

        /// <summary>
        /// Verifies Register returns 200 OK with a token and user payload on success.
        /// </summary>
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
            var controller = new AuthController(logger, logic, BuildConfig());

            var result = await controller.RegisterUser(new RegisterUserDto { Username = "bob", UserPassword = "password123", Role = "Admin" });

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            var payload = ok!.Value!;
            var tokenProp = payload.GetType().GetProperty("token");
            Assert.IsNotNull(tokenProp);
            var tokenVal = tokenProp!.GetValue(payload);
            Assert.IsNotNull(tokenVal);
            var userProp = payload.GetType().GetProperty("user");
            Assert.IsNotNull(userProp);
            var userVal = userProp!.GetValue(payload)!;
            var username = userVal.GetType().GetProperty("Username")!.GetValue(userVal) as string;
            var role = userVal.GetType().GetProperty("Role")!.GetValue(userVal) as string;
            Assert.AreEqual("bob", username);
            Assert.AreEqual("Admin", role);
        }

        /// <summary>
        /// Verifies Register maps logic-layer error to the correct HTTP status code.
        /// </summary>
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
            var controller = new AuthController(logger, logic, BuildConfig());

            var result = await controller.RegisterUser(new RegisterUserDto { Username = "bob", UserPassword = "password123" });

            var conflict = result as ObjectResult;
            Assert.IsNotNull(conflict);
            Assert.AreEqual(409, conflict!.StatusCode);
        }

        /// <summary>
        /// Verifies Login returns 200 OK with a token and user payload on success.
        /// </summary>
        [TestMethod]
        public async Task Login_ReturnsOk_WithTokenAndUser()
        {
            var logger = new TestLogger<AuthController>();
            var logic = new StubUserLogic
            {
                LoginUserResponse = new ApiResponse<IEnumerable<UserDetailDto>>
                {
                    IsSuccess = true,
                    Data = new[] { new UserDetailDto { Id = 1, Username = "alice", Role = "User" } },
                    ErrorCode = AppErrorCode.None
                }
            };
            var controller = new AuthController(logger, logic, BuildConfig());

            var result = await controller.Login(new LoginUserDto { Username = "alice", UserPassword = "p@ss" });

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            var payload = ok!.Value!;
            var tokenProp = payload.GetType().GetProperty("token");
            Assert.IsNotNull(tokenProp);
            var tokenVal = tokenProp!.GetValue(payload);
            Assert.IsNotNull(tokenVal);
            var userProp = payload.GetType().GetProperty("user");
            Assert.IsNotNull(userProp);
            var userVal = userProp!.GetValue(payload)!;
            var username = userVal.GetType().GetProperty("Username")!.GetValue(userVal) as string;
            Assert.AreEqual("alice", username);
        }

        /// <summary>
        /// Verifies Login maps logic-layer error to the correct HTTP status code.
        /// </summary>
        [TestMethod]
        public async Task Login_MapsError_FromLogic()
        {
            var logger = new TestLogger<AuthController>();
            var logic = new StubUserLogic
            {
                LoginUserResponse = new ApiResponse<IEnumerable<UserDetailDto>>
                {
                    IsSuccess = false,
                    ErrorMessage = "Invalid password",
                    ErrorCode = AppErrorCode.PasswordInvalid
                }
            };
            var controller = new AuthController(logger, logic, BuildConfig());

            var result = await controller.Login(new LoginUserDto { Username = "eve", UserPassword = "bad" });

            var unauthorized = result as ObjectResult;
            Assert.IsNotNull(unauthorized);
            Assert.AreEqual(401, unauthorized!.StatusCode);
        }
    }
}
