using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Models.Models;

namespace App.API.Test.Models
{
    /// <summary>
    /// Tests for shared ApiResponse model and error codes.
    /// </summary>
    [TestClass]
    public sealed class ApiResponseTests
    {
        /// <summary>
        /// Ensures default ApiResponse values are unset.
        /// </summary>
        [TestMethod]
        public void ApiResponse_Defaults()
        {
            var r = new ApiResponse<int>();
            Assert.AreEqual(0, r.Data);
            Assert.IsFalse(r.IsSuccess);
            Assert.IsNull(r.ErrorMessage);
            Assert.IsNull(r.ErrorCode);
        }

        /// <summary>
        /// Validates key enum integer values remain stable.
        /// </summary>
        [TestMethod]
        public void AppErrorCode_Values_AreStable()
        {
            Assert.AreEqual(0, (int)AppErrorCode.None);
            Assert.AreEqual(400, (int)AppErrorCode.Validation);
            Assert.AreEqual(404, (int)AppErrorCode.NotFound);
            Assert.AreEqual(409, (int)AppErrorCode.Conflict);
            Assert.AreEqual(500, (int)AppErrorCode.ServerError);
        }
    }
}
