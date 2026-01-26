using App.Models.Dtos;
using App.Models.Models;
using App.API.Logic.UserLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using App.API.Helpers;

namespace App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IUserLogic _userLogic;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthController(ILogger<AuthController> logger, IUserLogic userLogic, IConfiguration configuration)
        {
            _logger = logger;
            _userLogic = userLogic;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto userDetails)
        {
            var result = await _userLogic.RegisterUser(userDetails);

            if (!result.IsSuccess)
                return result.ToActionResult(this);

            var user = result.Data;
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User details missing." });

            var token = GenerateJwtToken(user.Id.ToString(), user.Username, user.Role ?? "User");

            return Ok(new
            {
                token,
                user = new { user.Username, user.Role }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            var userDetails = await _userLogic.LoginUser(dto);

            if (!userDetails.IsSuccess)
                return userDetails.ToActionResult(this);

            var user = userDetails.Data?.FirstOrDefault();
            if (user == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User details missing." });

            var token = GenerateJwtToken(user.Id.ToString(), user.Username, user.Role ?? "User");

            return Ok(new
            {
                token,
                user,
            });
        }

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