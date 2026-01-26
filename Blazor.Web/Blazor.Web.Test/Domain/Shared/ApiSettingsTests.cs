using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blazor.Web.Domain.Shared;

namespace App.Web.Test.Domain.Shared
{
    /// <summary>
    /// Tests verifying defaults and known route constants in <see cref="ApiSettings"/>.
    /// </summary>
    [TestClass]
    public class ApiSettingsTests
    {
        /// <summary>
        /// Asserts that default values and route segments are as expected.
        /// </summary>
        [TestMethod]
        public void Defaults_And_Routes_AreCorrect()
        {
            var settings = new ApiSettings();
            Assert.AreEqual(string.Empty, settings.AppApiBaseUrl);
            Assert.AreEqual("api/Auth", ApiSettings.Auth.Base);
            Assert.AreEqual("api/Auth/login", ApiSettings.Auth.Login);
            Assert.AreEqual("api/Auth/register", ApiSettings.Auth.Register);
        }
    }
}
