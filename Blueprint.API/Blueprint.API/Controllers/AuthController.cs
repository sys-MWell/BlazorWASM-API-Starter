using Template.Models.Dtos;
using Template.Models.Models;
using Blueprint.API.Logic.UserLogic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Blueprint.API.Helpers;
using Blueprint.API.Logic.Helpers;

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
        private readonly PasswordHasher<User> _passwordHasher = new();
        private readonly ITokenProvider _tokenProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="userLogic">User logic service.</param>
        /// <param name="token"></param>
        public AuthController(ILogger<AuthController> logger, IAuthLogic userLogic, ITokenProvider token)
        {
            _logger = logger;
            _userLogic = userLogic;
            _tokenProvider = token;
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

            return Ok(_tokenProvider.GenerateAuthResponse(user));
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

            return Ok(_tokenProvider.GenerateAuthResponse(user));
        }
    }
}