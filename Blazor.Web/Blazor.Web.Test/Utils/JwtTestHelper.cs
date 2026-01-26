using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blazor.Web.Test.Utils
{
    public static class JwtTestHelper
    {
        public static string CreateJwt(int userId, string username = "user", string? role = null, DateTimeOffset? expires = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username)
            };
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: (expires ?? DateTimeOffset.UtcNow.AddMinutes(5)).UtcDateTime
            );
            return handler.WriteToken(token);
        }
    }
}
