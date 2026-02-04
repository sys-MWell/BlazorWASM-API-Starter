using Template.Models.Dtos;
using Template.Models.Models;
using Blueprint.API.Logic.UserLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Blueprint.API.Helpers;

namespace Blueprint.API.Controllers
{
    /// <summary>
    /// Handles authentication operations such as user registration and login.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthLogic _userLogic;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="userLogic">User logic service.</param>
        /// <param name="configuration">Application configuration.</param>
        public AuthController(ILogger<AuthController> logger, IAuthLogic userLogic, IConfiguration configuration)
        {
            _logger = logger;
            _userLogic = userLogic;
            _configuration = configuration;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDetails">The registration details.</param>
        /// <returns>An <see cref="IActionResult"/> with an <see cref="AuthResponseDto"/> payload if successful.</returns>
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> RegisterUser([FromBody] RegisterUserDto userDetails)
        {
            var result = await _userLogic.RegisterUser(userDetails);

            if (!result.IsSuccess)
                return result.ToActionResult(this);

            var user = result.Data;
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User details missing." });

            var token = GenerateJwtToken(user.Id.ToString(), user.Username, user.Role ?? "User");

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserDetailDto { Id = user.Id, Username = user.Username, Role = user.Role }
            };

            return Ok(response);
        }

        /// <summary>
        /// Logs a user in.
        /// </summary>
        /// <param name="dto">The login credentials.</param>
        /// <returns>An <see cref="IActionResult"/> with an <see cref="AuthResponseDto"/> payload if successful.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginUserDto dto)
        {
            var userDetails = await _userLogic.LoginUser(dto);

            if (!userDetails.IsSuccess)
                return userDetails.ToActionResult(this);

            var user = userDetails.Data?.FirstOrDefault();
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User details missing." });

            var token = GenerateJwtToken(user.Id.ToString(), user.Username, user.Role ?? "User");

            var response = new AuthResponseDto
            {
                Token = token,
                User = new UserDetailDto { Id = user.Id, Username = user.Username, Role = user.Role }
            };

            return Ok(response);
        }

        /// <summary>
        /// Generates a JWT token for the specified user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="username">Username claim.</param>
        /// <param name="role">Role claim.</param>
        /// <returns>The signed JWT token string.</returns>
        private string GenerateJwtToken(string userId, string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}