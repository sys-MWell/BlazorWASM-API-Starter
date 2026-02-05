using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Logic.UserLogic;
using Blueprint.API.Repository.UserRepository;
using Template.Models.Dtos;
using Template.Models.Models;
using Blueprint.API.Logic.Helpers;

namespace Blueprint.API.Test.Logic
{
    /// <summary>
    /// Unit tests for <see cref="Blueprint.API.Logic.UserLogic.UserLogic"/> behavior.
    /// </summary>
    [TestClass]
    public sealed class UserLogicTests
    {
        private sealed class StubRepo : IAuthRepository
        {
            public ApiResponse<IEnumerable<UserDetailDto>>? GetByUsernameResponse { get; set; }
            public ApiResponse<UserDetailDto>? RegisterResponse { get; set; }
            public ApiResponse<string>? HashResponse { get; set; }

            public Task<ApiResponse<IEnumerable<UserDetailDto>>> GetUserByUsername(string username) => Task.FromResult(GetByUsernameResponse!);
            public Task<ApiResponse<UserDetailDto>> RegisterUser(User user) => Task.FromResult(RegisterResponse!);
            public Task<ApiResponse<string>> GetPasswordHashByUsername(string username) => Task.FromResult(HashResponse!);
        }

        /// <summary>
        /// Ensures repository AuthDetailsDto values map to UserDetailDto correctly.
        /// </summary>
        [TestMethod]
        public async Task GetUserByUsername_MapsDtos()
        {
            var repo = new StubRepo
            {
                GetByUsernameResponse = new ApiResponse<IEnumerable<UserDetailDto>>
                {
                    IsSuccess = true,
                    Data = new[] { new UserDetailDto { Id = 42, Username = "bob", Role = "Admin" } },
                    ErrorCode = AppErrorCode.None
                },
                HashResponse = new ApiResponse<string> { IsSuccess = true, Data = new Microsoft.AspNetCore.Identity.PasswordHasher<string>().HashPassword("bob", "x") }
            };
            var logic = new AuthLogic(repo, new PasswordVerifier());

            var res = await logic.GetUserByUsername("bob");
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(42, res.Data!.First().Id);
            Assert.AreEqual("bob", res.Data!.First().Username);
        }

        /// <summary>
        /// Returns UserNotFound when user lookup fails.
        /// </summary>
        [TestMethod]
        public async Task LoginUser_ReturnsUserNotFound_WhenRepoUserMissing()
        {
            var repo = new StubRepo
            {
                GetByUsernameResponse = new ApiResponse<IEnumerable<UserDetailDto>> { IsSuccess = false, ErrorCode = AppErrorCode.UserNotFound },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.UserNotFound }
            };
            var logic = new AuthLogic(repo, new PasswordVerifier());

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
            var repo = new StubRepo
            {
                GetByUsernameResponse = new ApiResponse<IEnumerable<UserDetailDto>> { IsSuccess = true, Data = new[] { new UserDetailDto { Id = 1, Username = "u" } } },
                HashResponse = new ApiResponse<string> { IsSuccess = true, Data = new Microsoft.AspNetCore.Identity.PasswordHasher<string>().HashPassword("u", "correct-password") }
            };
            var logic = new AuthLogic(repo, new PasswordVerifier());

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
            var repo = new StubRepo
            {
                GetByUsernameResponse = new ApiResponse<IEnumerable<UserDetailDto>> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 2, Username = "abc" } },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound }
            };
            var logic = new AuthLogic(repo, new PasswordVerifier());

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
            var repo = new StubRepo
            {
                GetByUsernameResponse = new ApiResponse<IEnumerable<UserDetailDto>> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                RegisterResponse = new ApiResponse<UserDetailDto> { IsSuccess = true, Data = new UserDetailDto { Id = 7, Username = "john", Role = "User" } },
                HashResponse = new ApiResponse<string> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound }
            };
            var logic = new AuthLogic(repo, new PasswordVerifier());

            var res = await logic.RegisterUser(new RegisterUserDto { Username = "john", UserPassword = "password123", Role = "User" });
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(7, res.Data!.Id);
            Assert.AreEqual("john", res.Data!.Username);
        }
    }
}
