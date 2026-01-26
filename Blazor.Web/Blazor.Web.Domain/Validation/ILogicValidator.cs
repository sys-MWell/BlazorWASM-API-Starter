using App.Models.Models;

namespace Blazor.Web.Domain.Validation
{
    /// <summary>
    /// Provides generic validation helpers for application logic to reduce repetitive null / success / token checks.
    /// </summary>
    public interface ILogicValidator
    {
        /// <summary>
        /// If the incoming response indicates failure, returns a mapped failure ApiResponse; otherwise passes through.
        /// </summary>
        ApiResponse<T> EnsureSuccess<T>(ApiResponse<T> response, AppErrorCode defaultErrorCode, string defaultMessage);

        /// <summary>
        /// Validates that a reference value is not null.
        /// </summary>
        ApiResponse<T> EnsureNotNull<T>(T? value, AppErrorCode errorCode, string message) where T : class;

        /// <summary>
        /// Creates a failure ApiResponse.
        /// </summary>
        ApiResponse<T> Fail<T>(AppErrorCode errorCode, string message);

        /// <summary>
        /// Returns true if token string is null or empty.
        /// </summary>
        bool IsTokenMissing(string? token);
    }
}
