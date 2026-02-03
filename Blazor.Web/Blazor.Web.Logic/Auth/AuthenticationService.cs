using Template.Models.Dtos;
using Blazor.Web.Logic.Services.Validation;
using Blazor.Web.Logic.User;

namespace Blazor.Web.Logic.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ICredentialValidator _credentialValidator;
        private readonly IUserLogic _userLogic;

        public AuthenticationService(ICredentialValidator credentialValidator, IUserLogic userLogic)
        {
            _credentialValidator = credentialValidator;
            _userLogic = userLogic;
        }

        public async Task<AuthenticationResult> LoginAsync(LoginUserDto user)
        {
            var result = new AuthenticationResult();

            var validationResult = _credentialValidator.ValidateCredentials(user.Username, user.UserPassword, forLogin: true);
            if (!validationResult.IsValid)
            {
                result.Errors = validationResult.Errors.Select(e => e.Message).ToList();
                return result;
            }

            var response = await _userLogic.UserLoginAsync(user);
            if (response.IsSuccess)
            {
                result.Success = true;
                return result;
            }

            result.ErrorCode = response.ErrorCode?.ToString();
            result.Errors.Add(response.ErrorMessage ?? "Login failed.");
            return result;
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterUserDto user)
        {
            var result = new AuthenticationResult();
            user.Username = user.Username?.Trim() ?? string.Empty;

            var validationResult = _credentialValidator.ValidateCredentials(user.Username, user.UserPassword, forLogin: false);
            if (!validationResult.IsValid)
            {
                result.Errors = validationResult.Errors.Select(e => e.Message).ToList();
                return result;
            }

            var response = await _userLogic.UserRegisterAsync(user);
            if (response.IsSuccess)
            {
                result.Success = true;
                return result;
            }

            result.ErrorCode = response.ErrorCode?.ToString();
            result.Errors.Add(response.ErrorMessage ?? "Registration failed.");
            return result;
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
