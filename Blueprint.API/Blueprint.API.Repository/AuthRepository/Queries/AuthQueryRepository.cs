using Dapper;
using System.Data;
using Blueprint.API.Repository.Shared;
using Blueprint.API.Repository.Helper;
using Blueprint.API.Models.Shared;
using Microsoft.Extensions.Logging;
using Template.Models.Dtos;
using Template.Models.Models;
using Template.Models;

namespace Blueprint.API.Repository.AuthRepository.Queries
{
    /// <summary>
    /// Implements query operations for authentication-related data retrieval using Dapper.
    /// Part of CQRS pattern - handles read operations only.
    /// </summary>
    public class AuthQueryRepository(DatabaseSettings databaseSettings, ILogger<AuthQueryRepository> logger)
        : RepositoryBase(databaseSettings), IAuthQueryRepository
    {
        private readonly ILogger<AuthQueryRepository> _logger = logger;

        /// <summary>
        /// Retrieves the stored password hash for a username.
        /// </summary>
        /// <param name="username">The username to look up.</param>
        /// <returns>An ApiResponse with the stored hash string.</returns>
        public async Task<ApiResponse<string>> GetPasswordHashByUsername(string username)
        {
            _logger.LogDebug("Executing stored procedure usp_GetPasswordHashByUsername for username: {Username}", username);

            DynamicParameters p = new();
            p.Add("@Username", username, dbType: DbType.String, size: 255);
            p.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            p.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var loginRow = await ExecuteQuery<User>("dbo.usp_GetPasswordHashByUsername", CommandType.StoredProcedure, p);
            var hash = loginRow?.UserPassword;

            string responseMessage = p.Get<string>("@ResponseMessage");
            int returnCode = p.Get<int>("@ErrorCode");

            if (returnCode != 0)
            {
                _logger.LogWarning("Database returned error code {ErrorCode} for GetPasswordHashByUsername. Message: {Message}", returnCode, responseMessage);
            }
            else
            {
                _logger.LogDebug("Successfully retrieved password hash for username: {Username}", username);
            }

            return ApiResponseRepoHelper.HandleDatabaseResponse<string>(returnCode, responseMessage, successData: hash, successMessage: hash ?? string.Empty);
        }

        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> containing user details.</returns>
        public async Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username)
        {
            _logger.LogDebug("Executing stored procedure usp_GetUserSummaryByUsername for username: {Username}", username);

            DynamicParameters userParameters = new();
            userParameters.Add("@Username", username, dbType: DbType.String, size: 255);
            userParameters.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            userParameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var user = await ExecuteQuery<User>("dbo.usp_GetUserSummaryByUsername", CommandType.StoredProcedure, userParameters);
            var dto = user?.ToUserDetailDto();

            string responseMessage = userParameters.Get<string>("@ResponseMessage");
            int returnCode = userParameters.Get<int>("@ErrorCode");

            if (returnCode != 0)
            {
                _logger.LogWarning("Database returned error code {ErrorCode} for GetUserByUsername. Message: {Message}", returnCode, responseMessage);
            }
            else
            {
                _logger.LogDebug("Successfully retrieved user details for username: {Username}", username);
            }

            return ApiResponseRepoHelper.HandleDatabaseResponse<UserDetailDto>(returnCode, responseMessage, successData: dto, successMessage: "User retrieved successfully.");
        }
    }
}
