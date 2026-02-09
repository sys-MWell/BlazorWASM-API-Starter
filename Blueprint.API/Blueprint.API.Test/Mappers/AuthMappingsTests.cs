using Microsoft.VisualStudio.TestTools.UnitTesting;
using Template.Models;
using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Test.Mappers
{
    /// <summary>
    /// Unit tests for <see cref="AuthMappings"/> extension methods.
    /// </summary>
    [TestClass]
    public sealed class AuthMappingsTests
    {
        /// <summary>
        /// Verifies that ToDomainUser creates a User with correct properties.
        /// </summary>
        [TestMethod]
        public void ToDomainUser_MapsCorrectly()
        {
            var dto = new RegisterUserDto { Username = "alice", UserPassword = "plain" };
            var hashedPassword = "hashed-password-123";

            var user = dto.ToDomainUser(hashedPassword);

            Assert.AreEqual("alice", user.Username);
            Assert.AreEqual("hashed-password-123", user.UserPassword);
            Assert.AreEqual("User", user.Role); // Default role
        }

        /// <summary>
        /// Verifies that ToRegisterDto creates a RegisterUserDto with correct properties.
        /// </summary>
        [TestMethod]
        public void ToRegisterDto_MapsCorrectly()
        {
            var user = new User { UserId = 1, Username = "bob", UserPassword = "secret", Role = "Admin" };

            var dto = user.ToRegisterDto();

            Assert.AreEqual("bob", dto.Username);
            Assert.AreEqual("secret", dto.UserPassword);
        }

        /// <summary>
        /// Verifies that ToUserDetailDto creates a UserDetailDto with correct properties.
        /// </summary>
        [TestMethod]
        public void ToUserDetailDto_MapsCorrectly()
        {
            var user = new User { UserId = 42, Username = "charlie", UserPassword = "hash", Role = "Manager" };

            var dto = user.ToUserDetailDto();

            Assert.AreEqual(42, dto.Id);
            Assert.AreEqual("charlie", dto.Username);
            Assert.AreEqual("Manager", dto.Role);
        }

        /// <summary>
        /// Verifies that ToDomainUser sets default role regardless of input.
        /// </summary>
        [TestMethod]
        public void ToDomainUser_AlwaysSetsDefaultRole()
        {
            var dto = new RegisterUserDto { Username = "test", UserPassword = "pass" };

            var user = dto.ToDomainUser("hash");

            Assert.AreEqual("User", user.Role);
        }
    }
}
