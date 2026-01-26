// App.Web.Domain/Aggregates/UserAggregate/User.cs
namespace Blazor.Web.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Represents a user in the system with a unique ID, username, password, and role.
    /// </summary>
    public class User
    {
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public string UserPassword { get; private set; }
        public string Role { get; private set; }

        /// <summary>
        /// Initialises a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <param name="username">The validated username of the user.</param>
        /// <param name="userPassword">The password of the user.</param>
        /// <param name="role">The role assigned to the user.</param>
        public User(int userId, string username, string userPassword, string role)
        {
            UserId = userId;
            Username = username;
            UserPassword = userPassword;
            Role = role;
        }
    }
}