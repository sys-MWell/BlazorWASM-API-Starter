using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Blueprint.API.Logic.UserLogic;
using Blueprint.API.Repository.AuthRepository.Commands;
using Blueprint.API.Repository.AuthRepository.Queries;
using Template.Models.Dtos;
using Template.Models.Models;
using Blueprint.API.Logic.Helpers;

namespace Blueprint.API.Test.Logic
{
    /// <summary>
    /// Unit tests for <see cref="AuthLogic"/> behavior using CQRS pattern.
    /// </summary>
    [TestClass]
    public sealed class UserLogicTests
    {
        private static readonly ILogger<AuthLogic> _nullLogger = NullLogger<AuthLogic>.Instance;

        /// <summary>
        /// Stub implementation of <see cref="IAuthQueryRepository"/> for testing query operations.
        /// </summary>
        private sealed class StubQueryRepo : IAuthQueryRepository
        {
            public ApiResponse<UserDetailDto>? GetByUsernameResponse { get; set; }
            public ApiResponse<string>? HashResponse { get; set; }

            public Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username) => Task.FromResult(GetByUsernameResponse!);
            public Task<ApiResponse<string>> GetPasswordHashByUsername(string username) => Task.FromResult(HashResponse!);
        }

        /// <summary>
        /// Stub implementation of <see cref="IAuthCommandRepository"/> for testing command operations.
        /// </summary>
        private sealed class StubCommandRepo : IAuthCommandRepository
        {
            public ApiResponse<UserDetailDto>? RegisterResponse { get; set; }

            public Task<ApiResponse<UserDetailDto>> RegisterUser(User user) => Task.FromResult(RegisterResponse!);
        }

        /// <summary>
        /// Ensures repository AuthDetailsDto values map to UserDetailDto correctly.
        /// </summary>
        [TestMethod]
        public async Task GetUserByUsername_MapsDtos()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = new UserDetailDto { Id = 42, Username = "bob", Role = "Admin" },
                    ErrorCode = AppErrorCode.None
                },
                HashResponse = new ApiResponse<string> { IsSuccess = true, Data = new Microsoft.AspNetCore.Identity.PasswordHasher<string>().HashPassword("bob", "x") }
            };
            var commandRepo = new StubCommandRepo();
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.GetUserByUsername("bob");
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(42, res.Data!.Id);
            Assert.AreEqual("bob", res.Data!.Username);
        }

        /// <summary>
        /// Returns UserNotFound when user lookup fails.
        /// </summary>
        [TestMethod]
        public async Task LoginUser_ReturnsUserNotFound_WhenRepoUserMissing()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorCode = AppErrorCode.UserNotFound },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.UserNotFound }
            };
            var commandRepo = new StubCommandRepo();
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.LoginUser(new LoginUserDto { Username = "x", UserPassword = "y" });
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.UserNotFound, res.ErrorCode);
        }

        /// <summary>
        /// Returns PasswordInvalid when provided password does not match hash.
        /// </summary>
        [TestMethod]
        public async Task LoginUser_InvalidPassword_Fails()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = "u" } },
                HashResponse = new ApiResponse<string> { IsSuccess = true, Data = new Microsoft.AspNetCore.Identity.PasswordHasher<string>().HashPassword("u", "correct-password") }
            };
            var commandRepo = new StubCommandRepo();
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.LoginUser(new LoginUserDto { Username = "u", UserPassword = "not-matching" });
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.PasswordInvalid, res.ErrorCode);
        }

        /// <summary>
        /// Validates weak registration inputs and returns validation error.
        /// </summary>
        [TestMethod]
        public async Task RegisterUser_ValidatesInputs()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound }
            };
            var commandRepo = new StubCommandRepo
            {
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 2, Username = "abc" } }
            };
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var weak = await logic.RegisterUser(new RegisterUserDto { Username = "ab", UserPassword = "123" });
            Assert.IsFalse(weak.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, weak.ErrorCode);
        }

        /// <summary>
        /// Maps successful registration repository response to UserDetailDto.
        /// </summary>
        [TestMethod]
        public async Task RegisterUser_Success_MapsUser()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound }
            };
            var commandRepo = new StubCommandRepo
            {
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 7, Username = "john", Role = "User" } }
            };
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.RegisterUser(new RegisterUserDto { Username = "john", UserPassword = "password123" });
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(7, res.Data!.Id);
            Assert.AreEqual("john", res.Data!.Username);
        }

        /// <summary>
        /// Returns success with user details when credentials are valid.
        /// </summary>
        [TestMethod]
        public async Task LoginUser_ValidCredentials_ReturnsUser()
        {
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<string>();
            var correctHash = hasher.HashPassword("validuser", "correctpassword");

            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = new UserDetailDto { Id = 99, Username = "validuser", Role = "Admin" }
                },
                HashResponse = new ApiResponse<string> { IsSuccess = true, Data = correctHash }
            };
            var commandRepo = new StubCommandRepo();
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.LoginUser(new LoginUserDto { Username = "validuser", UserPassword = "correctpassword" });

            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(99, res.Data!.Id);
            Assert.AreEqual("validuser", res.Data!.Username);
            Assert.AreEqual("Admin", res.Data!.Role);
        }

        /// <summary>
        /// Returns UserAlreadyExists error when attempting to register an existing user.
        /// </summary>
        [TestMethod]
        public async Task RegisterUser_UserAlreadyExists_ReturnsError()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = new UserDetailDto { Id = 1, Username = "existing", Role = "User" }
                },
                HashResponse = new ApiResponse<string> { IsSuccess = true, Data = "somehash" }
            };
            var commandRepo = new StubCommandRepo
            {
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 2, Username = "existing" } }
            };
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.RegisterUser(new RegisterUserDto { Username = "existing", UserPassword = "password123" });

            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.UserAlreadyExists, res.ErrorCode);
        }

        /// <summary>
        /// Returns Unauthorized when hash retrieval fails during login.
        /// </summary>
        [TestMethod]
        public async Task LoginUser_HashRetrievalFails_ReturnsUnauthorized()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto>
                {
                    IsSuccess = true,
                    Data = new UserDetailDto { Id = 1, Username = "user", Role = "User" }
                },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.ServerError }
            };
            var commandRepo = new StubCommandRepo();
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.LoginUser(new LoginUserDto { Username = "user", UserPassword = "pass" });

            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.Unauthorized, res.ErrorCode);
        }

        /// <summary>
        /// Returns validation error when username is too short.
        /// </summary>
        [TestMethod]
        public async Task RegisterUser_ShortUsername_ReturnsValidationError()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound }
            };
            var commandRepo = new StubCommandRepo
            {
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = "ab" } }
            };
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.RegisterUser(new RegisterUserDto { Username = "ab", UserPassword = "validpassword123" });

            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, res.ErrorCode);
            Assert.AreEqual("Invalid username", res.ErrorMessage);
        }

        /// <summary>
        /// Returns validation error when password is too short.
        /// </summary>
        [TestMethod]
        public async Task RegisterUser_ShortPassword_ReturnsValidationError()
        {
            var queryRepo = new StubQueryRepo
            {
                GetByUsernameResponse = new ApiResponse<UserDetailDto> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound }
            };
            var commandRepo = new StubCommandRepo
            {
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 1, Username = "validuser" } }
            };
            var logic = new AuthLogic(queryRepo, commandRepo, new PasswordVerifier(), _nullLogger);

            var res = await logic.RegisterUser(new RegisterUserDto { Username = "validuser", UserPassword = "short" });

            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, res.ErrorCode);
            Assert.AreEqual("Password too weak", res.ErrorMessage);
        }
    }
}
