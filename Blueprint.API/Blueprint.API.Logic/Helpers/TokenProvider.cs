using Blueprint.API.Logic.UserLogic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Template.Models.Dtos;

namespace Blueprint.API.Logic.Helpers
{
    public class TokenProvider : ITokenProvider
    {
        private readonly ILogger<TokenProvider> _logger;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger used to log token-related operations.</param>
        /// <param name="configuration">The application configuration containing JWT settings.</param>
        public TokenProvider(ILogger<TokenProvider> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">User details.</param>
        /// <returns>The signed JWT token string.</returns>
        public AuthResponseDto GenerateAuthResponse(UserDetailDto user)
        {
            try
            {
                var jwtSettings = _configuration.GetSection("Jwt");

                var keyValue = jwtSettings["Key"];
                if (string.IsNullOrWhiteSpace(keyValue))
                {
                    var ex = new InvalidOperationException("JWT key is missing or empty in configuration.");
                    _logger.LogError(ex, "Failed to generate JWT token due to missing key.");
                    throw ex;
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role ?? "Default")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var issuer = jwtSettings["Issuer"];
                var audience = jwtSettings["Audience"];
                var expiresInMinutesRaw = jwtSettings["ExpiresInMinutes"];

                if (!double.TryParse(expiresInMinutesRaw, out var expiresInMinutes))
                {
                    var ex = new InvalidOperationException("JWT expiration configuration 'ExpiresInMinutes' is invalid or missing.");
                    _logger.LogError(ex, "Failed to generate JWT token due to invalid expiration configuration.");
                    throw ex;
                }

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                    signingCredentials: creds);

                string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
                
                var authResponse = new AuthResponseDto
                {
                    Token = generatedToken,
                    User = new UserDetailDto { Id = user.Id, Username = user.Username, Role = user.Role }
                };

                return authResponse;
            }
            catch (Exception ex)
            {
                // Example log format provided by user; keeping appropriate error messaging for context
                _logger.LogError(ex, "Failed to generate JWT token.");
                throw;
            }
        }
    }
}
