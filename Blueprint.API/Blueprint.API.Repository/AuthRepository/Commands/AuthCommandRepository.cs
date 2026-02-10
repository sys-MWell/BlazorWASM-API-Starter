using Dapper;
using System.Data;
using Blueprint.API.Repository.Shared;
using Blueprint.API.Repository.Helper;
using Blueprint.API.Models.Shared;
using Microsoft.Extensions.Logging;
using Template.Models.Dtos;
using Template.Models.Models;
using Template.Models;

namespace Blueprint.API.Repository.AuthRepository.Commands
{
    /// <summary>
    /// Implements command operations for authentication-related data modifications using Dapper.
    /// Part of CQRS pattern - handles write operations only.
    /// </summary>
    public class AuthCommandRepository(DatabaseSettings databaseSettings, ILogger<AuthCommandRepository> logger)
        : RepositoryBase(databaseSettings), IAuthCommandRepository
    {
        private readonly ILogger<AuthCommandRepository> _logger = logger;

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="user">The domain user to register (contains hashed password).</param>
        /// <returns>An <see cref="ApiResponse{UserDetailDto}"/> with created user details.</returns>
        public async Task<ApiResponse<UserDetailDto>> RegisterUser(User user)
        {
            _logger.LogDebug("Executing stored procedure usp_RegisterUser for username: {Username}", user.Username);

            DynamicParameters userRegisterParameters = new();
            userRegisterParameters.Add("@Username", user.Username, dbType: DbType.String, size: 100);
            userRegisterParameters.Add("@UserPassword", user.UserPassword, dbType: DbType.String, size: 255);
            userRegisterParameters.Add("@Role", user.Role, dbType: DbType.String, size: 10);
            userRegisterParameters.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            userRegisterParameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var created = await ExecuteQuery<User>("dbo.usp_RegisterUser", CommandType.StoredProcedure, userRegisterParameters);
            var dto = created?.ToUserDetailDto();

            string responseMessage = userRegisterParameters.Get<string>("@ResponseMessage");
            int returnCode = userRegisterParameters.Get<int>("@ErrorCode");

            if (returnCode != 0)
            {
                _logger.LogWarning("Database returned error code {ErrorCode} for RegisterUser. Message: {Message}", returnCode, responseMessage);
            }
            else
            {
                _logger.LogInformation("Successfully registered user: {Username}", user.Username);
            }

            return ApiResponseRepoHelper.HandleDatabaseResponse<UserDetailDto>(returnCode, responseMessage, successData: dto, successMessage: "User registered successfully.");
        }
    }
}
