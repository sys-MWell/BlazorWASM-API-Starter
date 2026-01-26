namespace App.Models.Models
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public AppErrorCode? ErrorCode { get; set; }
    }

    public enum AppErrorCode
    {
        None = 0,
        NotFound = 404,
        Validation = 400,
        Unauthorized = 401,
        Conflict = 409,
        ServerError = 500,
        DatabaseError = 1001,
        PasswordInvalid = 2001,
        UserAlreadyExists = 2002,
        UserNotFound = 2003,
        TokenMissing = 3001,
        TokenExpired = 3002,
        LogicFailed = 3003,
        LoginFailed = 3004
    }
}