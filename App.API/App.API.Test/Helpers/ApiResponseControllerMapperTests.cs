using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using App.API.Helpers;
using App.Models.Models;

namespace App.API.Test.Helpers
{
    /// <summary>
    /// Tests for mapping <see cref="App.Models.Models.ApiResponse{T}"/> to <see cref="IActionResult"/>.
    /// </summary>
    [TestClass]
    public sealed class ApiResponseControllerMapperTests
    {
        private sealed class DummyController : ControllerBase { }

        private DummyController Controller() => new DummyController();

        /// <summary>
        /// Maps successful ApiResponse to 200 OK.
        /// </summary>
        [TestMethod]
        public void Success_ReturnsOk()
        {
            var resp = new ApiResponse<string> { IsSuccess = true, Data = "hi", ErrorCode = AppErrorCode.None };
            var result = resp.ToActionResult(Controller());
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        /// <summary>
        /// Maps NotFound error to 404.
        /// </summary>
        [TestMethod]
        public void NotFound_Returns404()
        {
            var resp = new ApiResponse<string> { IsSuccess = false, ErrorMessage = "x", ErrorCode = AppErrorCode.NotFound };
            var result = resp.ToActionResult(Controller());
            Assert.AreEqual(404, (result as ObjectResult)!.StatusCode);
        }

        /// <summary>
        /// Maps Validation error to 400.
        /// </summary>
        [TestMethod]
        public void Validation_Returns400()
        {
            var resp = new ApiResponse<string> { IsSuccess = false, ErrorMessage = "bad", ErrorCode = AppErrorCode.Validation };
            var result = resp.ToActionResult(Controller());
            Assert.AreEqual(400, (result as ObjectResult)!.StatusCode);
        }

        /// <summary>
        /// Maps Unauthorized error to 401.
        /// </summary>
        [TestMethod]
        public void Unauthorized_Returns401()
        {
            var resp = new ApiResponse<string> { IsSuccess = false, ErrorMessage = "bad", ErrorCode = AppErrorCode.Unauthorized };
            var result = resp.ToActionResult(Controller());
            Assert.AreEqual(401, (result as ObjectResult)!.StatusCode);
        }

        /// <summary>
        /// Maps UserAlreadyExists/Conflict to 409.
        /// </summary>
        [TestMethod]
        public void Conflict_Returns409()
        {
            var resp = new ApiResponse<string> { IsSuccess = false, ErrorMessage = "conflict", ErrorCode = AppErrorCode.UserAlreadyExists };
            var result = resp.ToActionResult(Controller());
            Assert.AreEqual(409, (result as ObjectResult)!.StatusCode);
        }

        /// <summary>
        /// Maps default/unhandled errors to 500.
        /// </summary>
        [TestMethod]
        public void Default_Returns500()
        {
            var resp = new ApiResponse<string> { IsSuccess = false, ErrorMessage = "err", ErrorCode = AppErrorCode.ServerError };
            var result = resp.ToActionResult(Controller());
            Assert.AreEqual(500, (result as ObjectResult)!.StatusCode);
        }
    }
}
