using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Logic.Validation;
using Template.Models.Dtos;

namespace Blueprint.API.Test.Validation
{
    /// <summary>
    /// Unit tests for <see cref="RegisterUserValidator"/>.
    /// </summary>
    [TestClass]
    public sealed class RegisterUserValidatorTests
    {
        private readonly RegisterUserValidator _validator = new();

        #region Username Tests

        /// <summary>
        /// Valid username passes validation.
        /// </summary>
        [TestMethod]
        public void Username_Valid_PassesValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Empty username fails validation.
        /// </summary>
        [TestMethod]
        public void Username_Empty_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Username"));
        }

        /// <summary>
        /// Username too short fails validation.
        /// </summary>
        [TestMethod]
        public void Username_TooShort_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "ab", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Username"));
        }

        /// <summary>
        /// Username with invalid characters fails validation.
        /// </summary>
        [TestMethod]
        public void Username_InvalidCharacters_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "user@name!", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "Username"));
        }

        /// <summary>
        /// Username with allowed special characters passes validation.
        /// </summary>
        [TestMethod]
        public void Username_AllowedCharacters_PassesValidation()
        {
            var dto = new RegisterUserDto { Username = "user_name-123", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            Assert.IsTrue(result.IsValid);
        }

        #endregion

        #region Password Tests

        /// <summary>
        /// Valid password passes validation.
        /// </summary>
        [TestMethod]
        public void Password_Valid_PassesValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            Assert.IsTrue(result.IsValid);
        }

        /// <summary>
        /// Empty password fails validation.
        /// </summary>
        [TestMethod]
        public void Password_Empty_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "UserPassword"));
        }

        /// <summary>
        /// Password too short fails validation.
        /// </summary>
        [TestMethod]
        public void Password_TooShort_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "Pass1!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "UserPassword"));
        }

        /// <summary>
        /// Password without uppercase fails validation.
        /// </summary>
        [TestMethod]
        public void Password_NoUppercase_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "password1!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage.Contains("uppercase")));
        }

        /// <summary>
        /// Password without lowercase fails validation.
        /// </summary>
        [TestMethod]
        public void Password_NoLowercase_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "PASSWORD1!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage.Contains("lowercase")));
        }

        /// <summary>
        /// Password without digit fails validation.
        /// </summary>
        [TestMethod]
        public void Password_NoDigit_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "Password!!" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage.Contains("number")));
        }

        /// <summary>
        /// Password without special character fails validation.
        /// </summary>
        [TestMethod]
        public void Password_NoSpecialChar_FailsValidation()
        {
            var dto = new RegisterUserDto { Username = "validuser", UserPassword = "Password12" };

            var result = _validator.Validate(dto);

            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.ErrorMessage.Contains("special character")));
        }

        #endregion

        #region Cascade Tests

        /// <summary>
        /// Validation stops at first error for username (CascadeMode.Stop).
        /// </summary>
        [TestMethod]
        public void Username_CascadeStop_ReturnsFirstError()
        {
            var dto = new RegisterUserDto { Username = "", UserPassword = "Password1!" };

            var result = _validator.Validate(dto);

            var usernameErrors = result.Errors.Where(e => e.PropertyName == "Username").ToList();
            Assert.AreEqual(1, usernameErrors.Count);
        }

        #endregion
    }
}
