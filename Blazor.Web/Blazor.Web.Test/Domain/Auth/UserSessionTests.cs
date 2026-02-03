using Blazor.Web.Domain.Auth;
using Template.Models.Dtos;

namespace App.Web.Test.Domain.Auth
{
    /// <summary>
    /// Tests for <see cref="UserSession"/> verifying set and clear behavior.
    /// </summary>
    [TestClass]
    public class UserSessionTests
    {
        /// <summary>
        /// Ensures that setting a user populates the session and Clear removes it.
        /// </summary>
        [TestMethod]
        public void SetUser_And_Clear_Works()
        {
            var session = new UserSession();
            var user = new UserDetailDto { Id = 42, Username = "alice" };
            session.SetUser(user);
            Assert.IsNotNull(session.CurrentUser);
            Assert.AreEqual(42, session.CurrentUser!.Id);
            session.Clear();
            Assert.IsNull(session.CurrentUser);
        }
    }
}
