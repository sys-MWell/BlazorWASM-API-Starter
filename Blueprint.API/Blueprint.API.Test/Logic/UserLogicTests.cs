using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Logic.UserLogic;
using Blueprint.API.Repository.UserRepository;
using Template.Models.Dtos;
using Template.Models.Models;

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
            public ApiResponse<IEnumerable<AuthDetailsDto>>? GetByUsernameResponse { get; set; }
            public ApiResponse<UserLoginResponseDto>? LoginResponse { get; set; }
            public ApiResponse<AuthDetailsDto>? RegisterResponse { get; set; }

            public Task<ApiResponse<IEnumerable<AuthDetailsDto>>> GetUserByUsername(string username) => Task.FromResult(GetByUsernameResponse!);
            public Task<ApiResponse<UserLoginResponseDto>> LoginUser(string userLogin) => Task.FromResult(LoginResponse!);
            public Task<ApiResponse<AuthDetailsDto>> RegisterUser(RegisterUserDto userRegister) => Task.FromResult(RegisterResponse!);
        }

        /// <summary>
        /// Ensures repository AuthDetailsDto values map to UserDetailDto correctly.
        /// </summary>
        [TestMethod]
        public async Task GetUserByUsername_MapsDtos()
        {
            var repo = new StubRepo
            {
                GetByUsernameResponse = new ApiResponse<IEnumerable<AuthDetailsDto>>
                {
                    IsSuccess = true,
                    Data = new[] { new AuthDetailsDto { UserId = 42, Username = "bob", Role = "Admin" } },
                    ErrorCode = AppErrorCode.None
                }
            };
            var logic = new UserLogic(repo);

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
                GetByUsernameResponse = new ApiResponse<IEnumerable<AuthDetailsDto>> { IsSuccess = false, ErrorCode = AppErrorCode.UserNotFound },
                LoginResponse = new ApiResponse<UserLoginResponseDto> { IsSuccess = false, ErrorCode = AppErrorCode.Unauthorized }
            };
            var logic = new UserLogic(repo);

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
                GetByUsernameResponse = new ApiResponse<IEnumerable<AuthDetailsDto>> { IsSuccess = true, Data = new[] { new AuthDetailsDto { UserId = 1, Username = "u" } } },
                // generate a real hashed password for a different plain text so verification will fail
                LoginResponse = new ApiResponse<UserLoginResponseDto> { IsSuccess = true, Data = new UserLoginResponseDto { UserId = 1, Username = "u", UserPassword = new Microsoft.AspNetCore.Identity.PasswordHasher<string>().HashPassword("u", "correct-password") } }
            };
            var logic = new UserLogic(repo);

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
                GetByUsernameResponse = new ApiResponse<IEnumerable<AuthDetailsDto>> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                RegisterResponse = new ApiResponse<AuthDetailsDto> { IsSuccess = true, Data = new AuthDetailsDto { UserId = 2, Username = "abc" } }
            };
            var logic = new UserLogic(repo);

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
                GetByUsernameResponse = new ApiResponse<IEnumerable<AuthDetailsDto>> { IsSuccess = false, ErrorCode = AppErrorCode.NotFound },
                RegisterResponse = new ApiResponse<AuthDetailsDto> { IsSuccess = true, Data = new AuthDetailsDto { UserId = 7, Username = "john", Role = "User" } }
            };
            var logic = new UserLogic(repo);

            var res = await logic.RegisterUser(new RegisterUserDto { Username = "john", UserPassword = "password123", Role = "User" });
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(7, res.Data!.Id);
            Assert.AreEqual("john", res.Data!.Username);
        }
    }
}
