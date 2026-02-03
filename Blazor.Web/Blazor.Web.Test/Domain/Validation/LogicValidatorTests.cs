using Blazor.Web.Domain.Validation;
using Template.Models.Models;

namespace App.Web.Test.Domain.Validation
{
    /// <summary>
    /// Tests for <see cref="LogicValidator"/> verifying success mapping, null checks, failure creation, and token checks.
    /// </summary>
    [TestClass]
    public class LogicValidatorTests
    {
        /// <summary>
        /// EnsureSuccess should pass through a successful response unchanged.
        /// </summary>
        [TestMethod]
        public void EnsureSuccess_PassesThrough_OnSuccess()
        {
            var validator = new LogicValidator();
            var response = new ApiResponse<int> { IsSuccess = true, Data = 5 };
            var ensured = validator.EnsureSuccess(response, AppErrorCode.LogicFailed, "fallback");
            Assert.IsTrue(ensured.IsSuccess);
            Assert.AreEqual(5, ensured.Data);
        }

        /// <summary>
        /// EnsureSuccess should map an unsuccessful response using provided fallback error details.
        /// </summary>
        [TestMethod]
        public void EnsureSuccess_MapsFailure_WithFallbacks()
        {
            var validator = new LogicValidator();
            var response = new ApiResponse<int> { IsSuccess = false };
            var ensured = validator.EnsureSuccess(response, AppErrorCode.Validation, "oops");
            Assert.IsFalse(ensured.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, ensured.ErrorCode);
            Assert.AreEqual("oops", ensured.ErrorMessage);
        }

        /// <summary>
        /// EnsureNotNull should return a success response when the value is present.
        /// </summary>
        [TestMethod]
        public void EnsureNotNull_ReturnsSuccess_WhenValuePresent()
        {
            var validator = new LogicValidator();
            var ensured = validator.EnsureNotNull("hi", AppErrorCode.Validation, "missing");
            Assert.IsTrue(ensured.IsSuccess);
            Assert.AreEqual("hi", ensured.Data);
            Assert.AreEqual(AppErrorCode.None, ensured.ErrorCode);
        }

        /// <summary>
        /// EnsureNotNull should return a failure response when the value is null.
        /// </summary>
        [TestMethod]
        public void EnsureNotNull_ReturnsFailure_WhenNull()
        {
            var validator = new LogicValidator();
            var ensured = validator.EnsureNotNull<string>(null, AppErrorCode.Validation, "missing");
            Assert.IsFalse(ensured.IsSuccess);
            Assert.AreEqual(AppErrorCode.Validation, ensured.ErrorCode);
            Assert.AreEqual("missing", ensured.ErrorMessage);
        }

        /// <summary>
        /// Fail should construct a failure ApiResponse with the provided code and message.
        /// </summary>
        [TestMethod]
        public void Fail_CreatesFailure()
        {
            var validator = new LogicValidator();
            var response = validator.Fail<int>(AppErrorCode.LoginFailed, "bad");
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual(AppErrorCode.LoginFailed, response.ErrorCode);
            Assert.AreEqual("bad", response.ErrorMessage);
        }

        /// <summary>
        /// IsTokenMissing returns true for null/whitespace and false for non-empty tokens.
        /// </summary>
        [TestMethod]
        public void IsTokenMissing_Works()
        {
            var validator = new LogicValidator();
            Assert.IsTrue(validator.IsTokenMissing(null));
            Assert.IsTrue(validator.IsTokenMissing(" "));
            Assert.IsFalse(validator.IsTokenMissing("x"));
        }
    }
}
