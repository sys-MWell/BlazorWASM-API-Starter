using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Logic.Validation;

namespace Blueprint.API.Test.Validation
{
    /// <summary>
    /// Unit tests for <see cref="ValidationConstants"/>.
    /// </summary>
    [TestClass]
    public sealed class ValidationConstantsTests
    {
        #region Username Constants

        /// <summary>
        /// Username min length is configured correctly.
        /// </summary>
        [TestMethod]
        public void Username_MinLength_IsThree()
        {
            Assert.AreEqual(3, ValidationConstants.Username.MinLength);
        }

        /// <summary>
        /// Username max length is configured correctly.
        /// </summary>
        [TestMethod]
        public void Username_MaxLength_IsFifty()
        {
            Assert.AreEqual(50, ValidationConstants.Username.MaxLength);
        }

        /// <summary>
        /// Username pattern is not null or empty.
        /// </summary>
        [TestMethod]
        public void Username_AllowedCharactersPattern_IsNotEmpty()
        {
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Username.AllowedCharactersPattern));
        }

        #endregion

        #region Password Constants

        /// <summary>
        /// Password min length is configured correctly.
        /// </summary>
        [TestMethod]
        public void Password_MinLength_IsEight()
        {
            Assert.AreEqual(8, ValidationConstants.Password.MinLength);
        }

        /// <summary>
        /// Password max length is configured correctly.
        /// </summary>
        [TestMethod]
        public void Password_MaxLength_Is128()
        {
            Assert.AreEqual(128, ValidationConstants.Password.MaxLength);
        }

        /// <summary>
        /// All password patterns are defined.
        /// </summary>
        [TestMethod]
        public void Password_AllPatterns_AreDefined()
        {
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.UppercasePattern));
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.LowercasePattern));
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.DigitPattern));
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.SpecialCharacterPattern));
        }

        /// <summary>
        /// All password error messages are defined.
        /// </summary>
        [TestMethod]
        public void Password_AllMessages_AreDefined()
        {
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.UppercaseMessage));
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.LowercaseMessage));
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.DigitMessage));
            Assert.IsFalse(string.IsNullOrEmpty(ValidationConstants.Password.SpecialCharacterMessage));
        }

        #endregion
    }
}
