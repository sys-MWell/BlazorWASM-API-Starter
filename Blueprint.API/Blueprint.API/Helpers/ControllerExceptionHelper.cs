using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
using Microsoft.Data.SqlClient;
using System.Collections;

namespace Blueprint.API.Helpers
{
    /// <summary>
    /// Provides helper methods for controller exception handling and standardized responses.
    /// </summary>
    public static class ControllerExceptionHelper
    {
        /// <summary>
        /// Executes a controller action and handles exceptions.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="logger">The logger instance for logging errors.</param>
        /// <param name="notFoundMessage">Optional message for NotFound responses.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the action or an error response.</returns>
        public static async Task<IActionResult> HandleRequestAsync<T>(
            Func<Task<T>> action,
            ILogger logger,
            string? notFoundMessage = null)
        {
            try
            {
                // Execute the action
                var result = await action();

                // Handle NotFound scenarios
                if (result == null || result.IsNullOrDefault() || IsEmptyEnumerable(result))
                {
                    return new NotFoundObjectResult(notFoundMessage ?? "The requested resource was not found.");
                }

                // Return Ok with the result    
                return new OkObjectResult(result);
            }
            catch (SqlException ex)
            {
                // Log SQL-specific errors
                logger.LogError(ex, "A SQL exception occurred.");
                return new ObjectResult("A database error occurred. Please check the ErrorLog table for stored procedure error details.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
            catch (Exception ex)
            {
                // Log general errors
                logger.LogError(ex, "An unexpected error occurred.");
                return new ObjectResult("An unexpected error occurred. Please try again later.")
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        private static bool IsEmptyEnumerable(object value)
        {
            if (value is string)
            {
                return false;
            }

            if (value is IEnumerable nonGeneric)
            {
                var enumerator = nonGeneric.GetEnumerator();
                try
                {
                    return !enumerator.MoveNext();
                }
                finally
                {
                    if (enumerator is IDisposable d)
                    {
                        d.Dispose();
                    }
                }
            }

            return false;
        }
    }
}
