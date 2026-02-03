using Template.Models.Dtos;

namespace Blazor.Web.Domain.Auth
{
    /// <summary>
    /// Holds the currently authenticated user's details for the lifetime of a circuit (and can be cleared on logout).
    /// </summary>
    public interface IUserSession
    {
        UserDetailDto? CurrentUser { get; }
        void SetUser(UserDetailDto user);
        void Clear();
    }
}
