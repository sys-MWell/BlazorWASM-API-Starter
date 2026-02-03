using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Blueprint.API.Controllers;

namespace Blueprint.API.Test.Controllers
{
    /// <summary>
    /// Contains unit tests for the <see cref="HealthCheckController"/>.
    /// </summary>
    [TestClass]
    public sealed class HealthCheckControllerTests
    {
        /// <summary>
        /// Verifies that the Get method on the HealthCheckController returns an OK result with a non-null payload.
        /// </summary>
        [TestMethod]
        public async Task Get_ReturnsOk_WithHealthPayload()
        {
            var controller = new HealthCheckController();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.Connection.LocalPort = 0; // let OS assign
            var result = await controller.Get();
            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            Assert.IsNotNull(ok!.Value);
        }
    }
}
