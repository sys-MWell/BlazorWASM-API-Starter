using Template.Models.Dtos;
using Template.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Web.Logic.User
{
    /// <summary>
    /// Defines user-related logic operations such as authentication and registration.
    /// </summary>
    public interface IUserLogic
    {
        /// <summary>
        /// Attempts to authenticate a user with the provided credentials.
        /// </summary>
        /// <param name="loginCredentials">The username and password payload.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing a populated <see cref="UserDetailDto"/> when successful;
        /// otherwise <c>IsSuccess = false</c> with an appropriate <see cref="AppErrorCode"/> and <c>ErrorMessage</c>.
        /// </returns>
        Task<ApiResponse<UserDetailDto>> UserLoginAsync(LoginUserDto loginCredentials);

        /// <summary>
        /// Registers a new user with the provided credentials.
        /// </summary>
        /// <param name="registerCredentials">The registration payload including username, password, and optional role.</param>
        /// <returns>
        /// An <see cref="ApiResponse{T}"/> containing the created <see cref="UserDetailDto"/> when successful;
        /// otherwise <c>IsSuccess = false</c> with an appropriate <see cref="AppErrorCode"/> and <c>ErrorMessage</c>.
        /// </returns>
        Task<ApiResponse<UserDetailDto>> UserRegisterAsync(RegisterUserDto registerCredentials);
    }
}
