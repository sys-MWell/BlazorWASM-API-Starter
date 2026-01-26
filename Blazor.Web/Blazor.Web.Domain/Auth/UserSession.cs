using App.Models.Dtos;

namespace Blazor.Web.Domain.Auth
{
    public class UserSession : IUserSession
    {
        public UserDetailDto? CurrentUser { get; private set; }
        public void SetUser(UserDetailDto user) => CurrentUser = user;
        public void Clear() => CurrentUser = null;
    }
}
