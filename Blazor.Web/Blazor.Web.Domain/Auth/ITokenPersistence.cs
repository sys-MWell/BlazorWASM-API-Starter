namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Abstraction for persisting auth tokens across browser refreshes.
    /// </summary>
    public interface ITokenPersistence
    {
        /// <summary>
        /// Persists the provided token to storage.
        /// </summary>
        /// <param name="token">The token to persist.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveTokenAsync(string token);

        /// <summary>
        /// Retrieves the persisted token from storage.
        /// </summary>
        /// <returns>The persisted token, or null if not found.</returns>
        Task<string?> GetTokenAsync();

        /// <summary>
        /// Removes the persisted token from storage.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ClearTokenAsync();
    }
}
