using Template.Models.Models; // ensures enum available

namespace Blueprint.API.Logic.Helpers
{
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
