using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Blueprint.API.Repository.Shared;
using Blueprint.API.Models.Shared;
using Microsoft.Data.SqlClient;

namespace Blueprint.API.Test.Repository
{
    /// <summary>
    /// Tests for common repository base behavior.
    /// </summary>
    [TestClass]
    public sealed class RepositoryBaseTests
    {
        private sealed class TestRepo : RepositoryBase
        {
            public TestRepo(DatabaseSettings s) : base(s) { }
            public SqlConnection Expose() => GetConnection();
        }

        /// <summary>
        /// Ensures connection created by base uses configured connection string.
        /// </summary>
        [TestMethod]
        public void GetConnection_UsesConfiguredConnectionString()
        {
            var settings = new DatabaseSettings { AppDbConnectionString = "Server=.;Database=tempdb;Trusted_Connection=True;Encrypt=False" };
            var repo = new TestRepo(settings);
            using var conn = repo.Expose();
            Assert.AreEqual(settings.AppDbConnectionString, conn.ConnectionString);
        }
    }
}
