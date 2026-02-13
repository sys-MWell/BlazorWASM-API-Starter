using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Logic.Helpers;
using Template.Models.Models;

namespace Blueprint.API.Test.Helpers
{
    /// <summary>
    /// Unit tests for <see cref="Guard"/> validation methods.
    /// </summary>
    [TestClass]
    public sealed class GuardTests
    {
        /// <summary>
        /// Non-empty string returns null (no error).
        /// </summary>
        [TestMethod]
        public void ValidateNotNullOrEmpty_Valid_ReturnsNull()
        {
            var result = Guard.ValidateNotNullOrEmpty<string>("value", "Field");

            Assert.IsNull(result);
        }

        /// <summary>
        /// Null string returns validation error with field name.
        /// </summary>
        [TestMethod]
        public void ValidateNotNullOrEmpty_Null_ReturnsError()
        {
            var result = Guard.ValidateNotNullOrEmpty<string>(null, "Email");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, result.ErrorCode);
            StringAssert.Contains(result.ErrorMessage!, "Email");
        }

        /// <summary>
        /// Empty string returns validation error.
        /// </summary>
        [TestMethod]
        public void ValidateNotNullOrEmpty_Empty_ReturnsError()
        {
            var result = Guard.ValidateNotNullOrEmpty<string>("", "Email");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }

        /// <summary>
        /// Whitespace string returns validation error.
        /// </summary>
        [TestMethod]
        public void ValidateNotNullOrEmpty_Whitespace_ReturnsError()
        {
            var result = Guard.ValidateNotNullOrEmpty<string>("   ", "Field");

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsSuccess);
        }
    }
}
