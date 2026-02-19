namespace Blazor.Web.Models.Models
{
    /// <summary>
    /// Represents a validation error with field name, error code, and human-readable message.
    /// </summary>
    /// <param name="Field">The name of the field that failed validation.</param>
    /// <param name="Code">A machine-readable error code identifying the validation rule that failed.</param>
    /// <param name="Message">A human-readable description of the validation error.</param>
    public record ValidationError(string Field, string Code, string Message);
}
