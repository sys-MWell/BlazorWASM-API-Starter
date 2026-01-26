using Blazor.Web.Models.Models;

namespace Blazor.Web.Logic.Services.Validation
{
    /// <summary>
    /// Provides credential validation logic for login or registration scenarios.
    /// </summary>
    public interface ICredentialValidator
    {
        /// <summary>
        /// Validates the supplied username and password.
        /// </summary>
        /// <param name="username">The username to validate. May be null or empty.</param>
        /// <param name="password">The password to validate. May be null or empty.</param>
        /// <param name="forLogin">True to validate rules applicable to login; false for broader (e.g. registration) validation.</param>
        /// <returns>A <see cref="CredentialValidationResult"/> containing validation status and any errors.</returns>
        CredentialValidationResult ValidateCredentials(string? username, string? password, bool forLogin = true);
    }
}
