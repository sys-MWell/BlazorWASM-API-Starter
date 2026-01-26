namespace Blazor.Web.Domain.Shared
{
    /// <summary>
    /// Provides configuration settings and known API route segments for the App application.
    /// </summary>
    public class ApiSettings
    {
        /// <summary>
        /// Gets or sets the base URL for the App API (e.g. https://localhost:5001/).
        /// </summary>
        /// <value>The base URL used to prefix all relative API endpoints.</value>
        public string AppApiBaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Contains API route segments related to user operations.
        /// </summary>
        public static class Auth
        {
            /// <summary>
            /// The base relative route segment for user-related API endpoints.
            /// </summary>
            public const string Base = "api/Auth";

            /// <summary>
            /// The relative route segment for the user login endpoint.
            /// </summary>
            public const string Login = Base + "/login";

            /// <summary>
            /// The relative route segment for the user registration endpoint.
            /// </summary>
            public const string Register = Base + "/register";
        }
    }
}