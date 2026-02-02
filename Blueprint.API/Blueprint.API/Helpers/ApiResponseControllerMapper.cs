using Microsoft.AspNetCore.Mvc;
using Template.Models.Models;

namespace Blueprint.API.Helpers
{
    /// <summary>
    /// Maps ApiResponse instances to IActionResult for API controllers.
    /// Presentation concern kept in API layer (not domain/logic).
    /// </summary>
    public static class ApiResponseControllerMapper
    {
        /// <summary>
        /// Converts an ApiResponse to an IActionResult with appropriate HTTP status code.
        /// </summary>
        public static IActionResult ToActionResult<T>(this ApiResponse<T> response, ControllerBase controller)
        {
            if (response.IsSuccess)
            {
                // Success -> 200 OK
                return controller.Ok(response.Data);
            }

            var message = response.ErrorMessage ?? "An error occurred";
            var code = response.ErrorCode ?? AppErrorCode.ServerError;

            return code switch
            {
                AppErrorCode.Validation => controller.BadRequest(new { error = message, code }),
                AppErrorCode.NotFound => controller.NotFound(new { error = message, code }),
                AppErrorCode.Unauthorized => controller.Unauthorized(new { error = message, code }),
                AppErrorCode.UserNotFound => controller.Unauthorized(new { error = message, code }),
                AppErrorCode.PasswordInvalid => controller.Unauthorized(new { error = message, code }),
                AppErrorCode.UserAlreadyExists => controller.Conflict(new { error = message, code }),
                AppErrorCode.ServerError => controller.StatusCode(StatusCodes.Status500InternalServerError, new { error = message, code }),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, new { error = message, code })
            };
        }
    }
}