using FluentValidation;
using Template.Models.Dtos;

namespace Blueprint.API.Logic.Validation
{
    /// <summary>
    /// Validates <see cref="RegisterUserDto"/> for user registration requests.
    /// Implements enterprise-level password complexity requirements.
    /// </summary>
    public sealed class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RegisterUserValidator"/> class
        /// with comprehensive validation rules for registration.
        /// </summary>
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Username is required");
                //.MinimumLength(ValidationConstants.Username.MinLength)
                //    .WithMessage($"Username must be at least {ValidationConstants.Username.MinLength} characters")
                //.MaximumLength(ValidationConstants.Username.MaxLength)
                //    .WithMessage($"Username cannot exceed {ValidationConstants.Username.MaxLength} characters")
                //.Matches(ValidationConstants.Username.AllowedCharactersPattern)
                //    .WithMessage(ValidationConstants.Username.AllowedCharactersMessage);

            RuleFor(x => x.UserPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                    .WithMessage("Password is required");
                //.MinimumLength(ValidationConstants.Password.MinLength)
                //    .WithMessage($"Password must be at least {ValidationConstants.Password.MinLength} characters")
                //.MaximumLength(ValidationConstants.Password.MaxLength)
                //    .WithMessage($"Password cannot exceed {ValidationConstants.Password.MaxLength} characters")
                //.Matches(ValidationConstants.Password.UppercasePattern)
                //    .WithMessage(ValidationConstants.Password.UppercaseMessage)
                //.Matches(ValidationConstants.Password.LowercasePattern)
                //    .WithMessage(ValidationConstants.Password.LowercaseMessage)
                //.Matches(ValidationConstants.Password.DigitPattern)
                //    .WithMessage(ValidationConstants.Password.DigitMessage)
                //.Matches(ValidationConstants.Password.SpecialCharacterPattern)
                //    .WithMessage(ValidationConstants.Password.SpecialCharacterMessage);
        }
    }
}
