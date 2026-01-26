using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.API.Models.Shared;

namespace App.API.Test.Domain
{
    /// <summary>
    /// Tests for simple domain settings models.
    /// </summary>
    [TestClass]
    public sealed class DatabaseSettingsTests
    {
        /// <summary>
        /// Verifies property roundtrip for connection string.
        /// </summary>
        [TestMethod]
        public void CanSetAndGet_ConnectionString()
        {
            var s = new DatabaseSettings { AppDbConnectionString = "cs" };
            Assert.AreEqual("cs", s.AppDbConnectionString);
        }
    }
}
