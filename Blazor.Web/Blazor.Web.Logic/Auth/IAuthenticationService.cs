using App.Models.Dtos;

namespace Blazor.Web.Logic.Auth
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> LoginAsync(LoginUserDto user);
        Task<AuthenticationResult> RegisterAsync(RegisterUserDto user);
        Task LogoutAsync();
    }
}
