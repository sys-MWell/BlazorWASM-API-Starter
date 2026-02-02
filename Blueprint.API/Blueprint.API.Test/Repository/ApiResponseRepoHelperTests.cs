using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Repository.Helper;
using Template.Models.Models;

namespace Blueprint.API.Test.Repository
{
    /// <summary>
    /// Tests for repository-layer ApiResponse handling helper.
    /// </summary>
    [TestClass]
    public sealed class ApiResponseRepoHelperTests
    {
        /// <summary>
        /// Returns success and data when return code indicates success.
        /// </summary>
        [TestMethod]
        public void HandleDatabaseResponse_Success_ReturnsData()
        {
            var res = ApiResponseRepoHelper.HandleDatabaseResponse(0, "OK", successData: 123, successMessage: null);
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual(123, res.Data);
            Assert.AreEqual(AppErrorCode.None, res.ErrorCode);
        }

        /// <summary>
        /// Returns message as data for string type when provided.
        /// </summary>
        [TestMethod]
        public void HandleDatabaseResponse_Success_MessageForString()
        {
            var res = ApiResponseRepoHelper.HandleDatabaseResponse<string>(0, "OK", successData: null, successMessage: "Saved");
            Assert.IsTrue(res.IsSuccess);
            Assert.AreEqual("Saved", res.Data);
        }

        /// <summary>
        /// Maps non-zero return code to error ApiResponse.
        /// </summary>
        [TestMethod]
        public void HandleDatabaseResponse_Error_MapsCode()
        {
            var res = ApiResponseRepoHelper.HandleDatabaseResponse<object>(1, "Bad");
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.ServerError, res.ErrorCode);
            Assert.AreEqual("Bad", res.ErrorMessage);
        }
    }
}
