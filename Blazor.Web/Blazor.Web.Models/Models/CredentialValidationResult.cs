namespace Blazor.Web.Models.Models
{
    public class CredentialValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<ValidationError> Errors { get; } = new();
    }
}