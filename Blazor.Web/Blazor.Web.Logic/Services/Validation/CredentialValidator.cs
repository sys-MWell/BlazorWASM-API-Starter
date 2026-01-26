using Blazor.Web.Models.Models;

namespace Blazor.Web.Logic.Services.Validation
{
    /// <summary>
    /// Validates username and password credentials for login or registration scenarios.
    /// Aggregates validation errors into a <see cref="CredentialValidationResult"/>.
    /// </summary>
    public class CredentialValidator : ICredentialValidator
    {
        /// <summary>
        /// Validates the supplied username and password.
        /// </summary>
        /// <param name="username">The username to validate. May be null or whitespace.</param>
        /// <param name="password">The password to validate. May be null or whitespace.</param>
        /// <param name="forLogin">
        /// True to apply only the rules required for login (e.g. existence / basic checks),
        /// false to apply extended rules (e.g. complexity) suitable for registration.
        /// </param>
        /// <returns>
        /// A <see cref="CredentialValidationResult"/> containing any validation errors.
        /// If <c>Errors</c> is empty, the credentials are considered valid.
        /// </returns>
        public CredentialValidationResult ValidateCredentials(string? username, string? password, bool forLogin = true)
        {
            var result = new CredentialValidationResult();

            ValidateUsername(username, result, forLogin);
            ValidatePassword(password, result, forLogin);
            return result;
        }

        /// <summary>
        /// Performs validation on the username and adds any encountered errors to the provided result.
        /// </summary>
        /// <param name="username">The username to validate.</param>
        /// <param name="result">The aggregate result container to append errors to.</param>
        /// <param name="forLogin">Indicates whether validation is for login (currently unused for username rules).</param>
        private static void ValidateUsername(string? username, CredentialValidationResult result, bool forLogin)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                result.Errors.Add(new("Username", "Required", "Username is required"));
                return;
            }

            var trimmed = username.Trim();
            if (trimmed.Length < 4 || trimmed.Length > 20)
            {
                result.Errors.Add(new("Username", "Length", "Username must be between 4 and 20 characters long"));
                return;
            }
        }

        /// <summary>
        /// Performs validation on the password and adds any encountered errors to the provided result.
        /// Extended complexity rules are currently commented out.
        /// </summary>
        /// <param name="password">The password to validate.</param>
        /// <param name="result">The aggregate result container to append errors to.</param>
        /// <param name="forLogin">Indicates whether validation is for login (skips extended complexity when true).</param>
        private static void ValidatePassword(string? password, CredentialValidationResult result, bool forLogin)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                result.Errors.Add(new("Password", "Required", "Password is required"));
                return;
            }
            //var trimmed = password.Trim();
            //if (trimmed.Length < 8 || trimmed.Length > 100)
            //{
            //    result.Errors.Add(new("Password", "Length", "Password must be between 8 and 100 characters long"));
            //}
            //if (!forLogin)
            //{
            //    if (!trimmed.Any(char.IsUpper))
            //    {
            //        result.Errors.Add(new("Password", "Uppercase", "Password must contain at least one uppercase letter"));
            //    }
            //    if (!trimmed.Any(char.IsLower))
            //    {
            //        result.Errors.Add(new("Password", "Lowercase", "Password must contain at least one lowercase letter"));
            //    }
            //    if (!trimmed.Any(char.IsDigit))
            //    {
            //        result.Errors.Add(new("Password", "Digit", "Password must contain at least one digit"));
            //    }
            //    if (!trimmed.Any(ch => !char.IsLetterOrDigit(ch)))
            //    {
            //        result.Errors.Add(new("Password", "SpecialCharacter", "Password must contain at least one special character"));
            //    }
            //}
        }
    }
}
