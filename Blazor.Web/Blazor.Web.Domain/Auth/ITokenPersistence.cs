namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Abstraction for persisting auth tokens across browser refreshes.
    /// </summary>
    public interface ITokenPersistence
    {
        Task SaveTokenAsync(string token);
        Task<string?> GetTokenAsync();
        Task ClearTokenAsync();
    }
}
