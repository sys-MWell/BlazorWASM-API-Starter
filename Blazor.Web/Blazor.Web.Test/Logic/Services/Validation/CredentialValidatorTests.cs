using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blazor.Web.Logic.Services.Validation;

namespace App.Web.Test.Logic.Services.Validation
{
    /// <summary>
    /// Tests for <see cref="CredentialValidator"/> covering required fields and minimal valid login scenario.
    /// </summary>
    [TestClass]
    public class CredentialValidatorTests
    {
        /// <summary>
        /// When both username and password are missing, two validation errors are returned.
        /// </summary>
        [TestMethod]
        public void ValidateCredentials_BothMissing_ReturnsTwoErrors()
        {
            var validator = new CredentialValidator();
            var result = validator.ValidateCredentials(null, null);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(2, result.Errors.Count);
            Assert.IsTrue(result.Errors.Any(e => e.Field == "Username" && e.Code == "Required"));
            Assert.IsTrue(result.Errors.Any(e => e.Field == "Password" && e.Code == "Required"));
        }

        /// <summary>
        /// A username shorter than 4 characters fails validation.
        /// </summary>
        [TestMethod]
        public void ValidateCredentials_UsernameTooShort()
        {
            var validator = new CredentialValidator();
            var result = validator.ValidateCredentials("abc", "pass");
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("Username", result.Errors[0].Field);
        }

        /// <summary>
        /// Minimal valid login: username length is valid and password non-empty.
        /// </summary>
        [TestMethod]
        public void ValidateCredentials_ValidMinimalLogin()
        {
            var validator = new CredentialValidator();
            var result = validator.ValidateCredentials("john1", "p");
            Assert.IsTrue(result.IsValid);
        }
    }
}
