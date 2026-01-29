using App.Models.Dtos;
using App.Models.Models;

namespace Blazor.Web.Repository.User
{
    public interface IUserRepository
    {
        /// <summary>
        /// Authenticates a user with the provided login credentials.
        /// </summary>
        /// <param name="loginCredentials">The user's login credentials containing username/email and password.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an <see cref="ApiResponse{T}"/> 
        /// with <see cref="AuthResponseDto"/> data if authentication is successful; otherwise, an error response.
        /// </returns>
        Task<ApiResponse<AuthResponseDto>> UserLoginAsync(LoginUserDto loginCredentials);

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <param name="registerCredentials">The user registration information including email, password, and profile details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an <see cref="ApiResponse{T}"/> 
        /// with <see cref="AuthResponseDto"/> data if registration is successful; otherwise, an error response.
        /// </returns>
        Task<ApiResponse<AuthResponseDto>> UserRegisterAsync(RegisterUserDto registerCredentials);
    }
}