using App.Models.Models;

namespace Blazor.Web.Domain.Validation
{
    /// <summary>
    /// Concrete implementation of <see cref="ILogicValidator"/> providing reusable helpers
    /// for application logic validations (success mapping, null checks, failure creation, token checks).
    /// </summary>
    public class LogicValidator : ILogicValidator
    {
        /// <summary>
        /// Passes through a successful <see cref="ApiResponse{T}"/>; if unsuccessful, maps it to a new failure response
        /// using provided default error details when originals are missing.
        /// </summary>
        /// <typeparam name="T">The data type contained in the API response.</typeparam>
        /// <param name="response">The incoming response to validate.</param>
        /// <param name="defaultErrorCode">Fallback error code if the original response has none.</param>
        /// <param name="defaultMessage">Fallback error message if the original response has none.</param>
        /// <returns>
        /// The original response when successful; otherwise a new failure <see cref="ApiResponse{T}"/> populated with
        /// original or fallback error information.
        /// </returns>
        public ApiResponse<T> EnsureSuccess<T>(ApiResponse<T> response, AppErrorCode defaultErrorCode, string defaultMessage)
        {
            if (response.IsSuccess) return response;
            return new ApiResponse<T>
            {
                IsSuccess = false,
                ErrorCode = response.ErrorCode ?? defaultErrorCode,
                ErrorMessage = response.ErrorMessage ?? defaultMessage
            };
        }

        /// <summary>
        /// Validates that the supplied reference value is not null, returning a success response containing the value,
        /// or a failure response if null.
        /// </summary>
        /// <typeparam name="T">The reference type being validated.</typeparam>
        /// <param name="value">The value to check for null.</param>
        /// <param name="errorCode">The error code to use when the value is null.</param>
        /// <param name="message">The error message to use when the value is null.</param>
        /// <returns>
        /// A successful <see cref="ApiResponse{T}"/> containing the value when not null; otherwise a failure response.
        /// </returns>
        public ApiResponse<T> EnsureNotNull<T>(T? value, AppErrorCode errorCode, string message) where T : class
        {
            if (value is not null)
            {
                return new ApiResponse<T>
                {
                    IsSuccess = true,
                    Data = value,
                    ErrorCode = AppErrorCode.None
                };
            }
            return Fail<T>(errorCode, message);
        }

        /// <summary>
        /// Creates a failure <see cref="ApiResponse{T}"/> populated with the specified error code and message.
        /// </summary>
        /// <typeparam name="T">The expected data type of the response.</typeparam>
        /// <param name="errorCode">The application-specific error code.</param>
        /// <param name="message">A human-readable error message.</param>
        /// <returns>A failure <see cref="ApiResponse{T}"/>.</returns>
        public ApiResponse<T> Fail<T>(AppErrorCode errorCode, string message)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                ErrorMessage = message
            };
        }

        /// <summary>
        /// Determines whether a token string is missing (null, empty, or whitespace).
        /// </summary>
        /// <param name="token">The token value to inspect.</param>
        /// <returns><c>true</c> if the token is null, empty, or whitespace; otherwise <c>false</c>.</returns>
        public bool IsTokenMissing(string? token) => string.IsNullOrWhiteSpace(token);
    }
}
