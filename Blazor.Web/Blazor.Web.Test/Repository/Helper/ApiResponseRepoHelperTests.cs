using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blazor.Web.Repository.Shared;
using App.Models.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace App.Web.Test.Repository.Helper
{
    /// <summary>
    /// Tests for <see cref="ApiResponseRepoHelper"/> ensuring correct success and error handling.
    /// </summary>
    [TestClass]
    public class ApiResponseRepoHelperTests
    {
        private class Sample
        {
            public int Id { get; set; }
            public string? Name { get; set; }
        }

        /// <summary>
        /// A successful 200 response with valid JSON should deserialize into the target type.
        /// </summary>
        [TestMethod]
        public async Task HandleHttpResponseAsync_Success_Deserializes()
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"id\":1,\"name\":\"n\"}", System.Text.Encoding.UTF8, "application/json")
            };
            var res = await ApiResponseRepoHelper.HandleHttpResponseAsync<Sample>(message);
            Assert.IsTrue(res.IsSuccess);
            Assert.IsNotNull(res.Data);
            Assert.AreEqual(1, res.Data!.Id);
        }

        /// <summary>
        /// A successful 200 response with invalid JSON should produce a failure with a deserialization error message.
        /// </summary>
        [TestMethod]
        public async Task HandleHttpResponseAsync_Success_BadJson_SetsFailure()
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("not json", System.Text.Encoding.UTF8, "application/json")
            };
            var res = await ApiResponseRepoHelper.HandleHttpResponseAsync<Sample>(message);
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual("Failed to deserialise response.", res.ErrorMessage);
        }

        /// <summary>
        /// A non-success response should map HTTP status to AppErrorCode and surface the content as the error message.
        /// </summary>
        [TestMethod]
        public async Task HandleHttpResponseAsync_Failure_MapsErrorCode_And_Message()
        {
            var message = new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Nope", System.Text.Encoding.UTF8, "text/plain")
            };
            var res = await ApiResponseRepoHelper.HandleHttpResponseAsync<Sample>(message);
            Assert.IsFalse(res.IsSuccess);
            Assert.AreEqual(AppErrorCode.Unauthorized, res.ErrorCode);
            Assert.AreEqual("Nope", res.ErrorMessage);
        }
    }
}
