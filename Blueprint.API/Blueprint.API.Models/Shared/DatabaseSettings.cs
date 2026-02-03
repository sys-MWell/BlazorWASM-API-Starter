namespace Blueprint.API.Models.Shared
{
    /// <summary>
    /// Provides database connection settings for the API repositories.
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Gets or sets the connection string for the application database.
        /// </summary>
        public string AppDbConnectionString { get; set; } = string.Empty;
    }
}
