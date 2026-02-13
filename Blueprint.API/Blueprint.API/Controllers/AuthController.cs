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
    [Produces("application/json")]
    [Consumes("application/json")]
    [Tags("Authentication")]
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
        /// <param name="token">The token provider for generating JWT tokens.</param>
        public AuthController(ILogger<AuthController> logger, IAuthLogic userLogic, ITokenProvider token)
        {
            _logger = logger;
            _userLogic = userLogic;
            _tokenProvider = token;
        }

        /// <summary>
        /// Registers a new user account.
        /// </summary>
        /// <remarks>
        /// Creates a new user account with the provided credentials. Upon successful registration,
        /// returns a JWT token that can be used for subsequent authenticated requests.
        /// 
        /// **Password Requirements:**
        /// - Minimum 8 characters
        /// - At least one uppercase letter
        /// - At least one lowercase letter
        /// - At least one number
        /// </remarks>
        /// <param name="userDetails">The registration details including username, email, and password.</param>
        /// <returns>An authentication response containing the JWT token and user details.</returns>
        /// <response code="200">Registration successful. Returns JWT token and user info.</response>
        /// <response code="400">Invalid registration data or validation errors.</response>
        /// <response code="409">Username or email already exists.</response>
        /// <response code="500">Internal server error during registration.</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> RegisterUser([FromBody] RegisterUserDto userDetails)
        {
            _logger.LogInformation("Registration attempt for username: {Username}", userDetails.Username);

            var result = await _userLogic.RegisterUser(userDetails);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Registration failed for username: {Username}. Reason: {ErrorMessage}", userDetails.Username, result.ErrorMessage);
                return result.ToActionResult(this);
            }

            var user = result.Data;
            if (user == null)
            {
                _logger.LogError("Registration succeeded but user details missing for username: {Username}", userDetails.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User details missing." });
            }

            _logger.LogInformation("Registration successful for username: {Username}, UserId: {UserId}", user.Username, user.Id);
            return Ok(_tokenProvider.GenerateAuthResponse(user));
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <remarks>
        /// Validates the provided credentials against stored user data. Upon successful authentication,
        /// returns a JWT token that should be included in the Authorization header for protected endpoints.
        /// 
        /// **Usage:**
        /// ```
        /// Authorization: Bearer {token}
        /// ```
        /// 
        /// The token expires after the configured duration (default: 60 minutes).
        /// </remarks>
        /// <param name="dto">The login credentials containing username and password.</param>
        /// <returns>An authentication response containing the JWT token and user details.</returns>
        /// <response code="200">Login successful. Returns JWT token and user info.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="401">Invalid username or password.</response>
        /// <response code="500">Internal server error during authentication.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginUserDto dto)
        {
            _logger.LogInformation("Login attempt for username: {Username}", dto.Username);

            var userDetails = await _userLogic.LoginUser(dto);

            if (!userDetails.IsSuccess)
            {
                _logger.LogWarning("Login failed for username: {Username}. Reason: {ErrorMessage}", dto.Username, userDetails.ErrorMessage);
                return userDetails.ToActionResult(this);
            }

            var user = userDetails.Data;
            if (user == null)
            {
                _logger.LogError("Login succeeded but user details missing for username: {Username}", dto.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User details missing." });
            }

            _logger.LogInformation("Login successful for username: {Username}, UserId: {UserId}", user.Username, user.Id);
            return Ok(_tokenProvider.GenerateAuthResponse(user));
        }
    }
}