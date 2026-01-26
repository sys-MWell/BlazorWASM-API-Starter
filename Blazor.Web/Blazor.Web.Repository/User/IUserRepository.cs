using App.Models.Dtos;
using App.Models.Models;

namespace Blazor.Web.Repository.User
{
    public interface IUserRepository
    {
        Task<ApiResponse<AuthResponseDto>> UserLoginAsync(LoginUserDto loginCredentials);
        //Task<ApiResponse<UserDetailDto>> UserRegisterAsync(RegisterUserDto registerCredentials);
    }
}