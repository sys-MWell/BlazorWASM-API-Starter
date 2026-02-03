using Template.Models.Models; // ensures enum available

namespace Blueprint.API.Logic.Helpers
{
    /// <summary>
    /// Provides helper methods to create and map `ApiResponse` instances in the logic layer.
    /// </summary>
    public static class ApiResponseLogicHelper
    {
        /// <summary>
        /// Creates an error response with the specified error message and code.
        /// </summary>
        public static ApiResponse<T> CreateErrorResponse<T>(string errorMessage, AppErrorCode errorCode)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
        }

        /// <summary>
        /// Handles a repository response and maps it to an ApiResponse.
        /// </summary>
        /// <typeparam name="T">The response data type.</typeparam>
        /// <param name="repositoryResponse">The repository response to evaluate.</param>
        /// <param name="successData">The data to return when successful.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> with the appropriate success or error state.</returns>
        public static ApiResponse<T> HandleRepositoryResponse<T>(ApiResponse<T> repositoryResponse, T successData)
        {
            if (!repositoryResponse.IsSuccess)
            {
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    ErrorMessage = repositoryResponse.ErrorMessage,
                    ErrorCode = repositoryResponse.ErrorCode
                };
            }

            return new ApiResponse<T>
            {
                IsSuccess = true,
                Data = successData,
                ErrorCode = AppErrorCode.None
            };
        }

        /// <summary>
        /// Handles a repository response and maps it to an ApiResponse with a success message.
        /// </summary>
        /// <param name="repositoryResponse">The repository response to evaluate.</param>
        /// <param name="successMessage">The message to return on success.</param>
        /// <returns>An <see cref="ApiResponse{String}"/> representing the result.</returns>
        public static ApiResponse<string> HandleRepositoryResponse(ApiResponse<string> repositoryResponse, string successMessage)
        {
            if (!repositoryResponse.IsSuccess)
            {
                return new ApiResponse<string>
                {
                    IsSuccess = false,
                    ErrorMessage = repositoryResponse.ErrorMessage,
                    ErrorCode = repositoryResponse.ErrorCode
                };
            }

            return new ApiResponse<string>
            {
                IsSuccess = true,
                Data = successMessage,
                ErrorCode = AppErrorCode.None
            };
        }
    }
}
