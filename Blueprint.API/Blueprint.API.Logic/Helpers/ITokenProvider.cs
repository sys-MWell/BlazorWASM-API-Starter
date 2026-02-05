using System;
using Template.Models.Dtos;

namespace Blueprint.API.Logic.Helpers
{
    /// <summary>
    /// Provides functionality to generate JSON Web Tokens (JWT) for authenticated users.
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">User details.</param>
        /// <returns>The signed JWT token string.</returns>
        AuthResponseDto GenerateAuthResponse(UserDetailDto user);
    }
}
