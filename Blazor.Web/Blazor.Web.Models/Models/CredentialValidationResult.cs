namespace Blazor.Web.Models.Models
{
    /// <summary>
    /// Represents the result of credential validation, containing any validation errors encountered.
    /// </summary>
    public class CredentialValidationResult
    {
        /// <summary>
        /// Gets a value indicating whether the credentials are valid (no errors).
        /// </summary>
        public bool IsValid => Errors.Count == 0;

        /// <summary>
        /// Gets the collection of validation errors encountered during credential validation.
        /// </summary>
        public List<ValidationError> Errors { get; } = new();
    }
}