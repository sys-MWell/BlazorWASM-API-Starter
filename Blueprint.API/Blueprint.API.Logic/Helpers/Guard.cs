using Template.Models.Models;

namespace Blueprint.API.Logic.Helpers
{
    /// <summary>
    /// Provides guard clause validation methods that return ApiResponse errors when validation fails.
    /// Use these methods for early-return validation patterns in the logic layer.
    /// For complex DTO validation (e.g., registration), use FluentValidation validators.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Validates that a string value is not null, empty, or whitespace.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="value">The value to validate.</param>
        /// <param name="fieldName">The name of the field being validated (for error message).</param>
        /// <returns>An error ApiResponse if validation fails; null if valid.</returns>
        public static ApiResponse<T>? ValidateNotNullOrEmpty<T>(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ApiResponseLogicHelper.CreateErrorResponse<T>(
                    $"{fieldName} is required",
                    AppErrorCode.Validation);
            }

            return null;
        }
    }
}
