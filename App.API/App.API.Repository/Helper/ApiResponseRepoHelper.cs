using App.Models.Models;

namespace App.API.Repository.Helper
{
    public static class ApiResponseRepoHelper
    {
        public static ApiResponse<T> HandleDatabaseResponse<T>(int returnCode, string responseMessage, T? successData = default, string? successMessage = null)
        {
            // Stored procedures: 0 = success, 1 = error
            if (returnCode == 1)
            {
                return new ApiResponse<T>
                {
                    IsSuccess = false,
                    ErrorMessage = responseMessage,
                    ErrorCode = MapReturnCodeToError(returnCode)
                };
            }

            return new ApiResponse<T>
            {
                IsSuccess = true,
                Data = successData != null ? successData :
                    typeof(T) == typeof(string) && successMessage != null
                        ? (T)(object)successMessage
                        : default,
                ErrorCode = AppErrorCode.None
            };
        }

        private static AppErrorCode MapReturnCodeToError(int returnCode)
        {
            return returnCode switch
            {
                1 => AppErrorCode.ServerError, // generic SP error
                _ => AppErrorCode.ServerError
            };
        }
    }
}