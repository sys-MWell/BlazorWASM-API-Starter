using Dapper;
using System.Data;
using Blueprint.API.Repository.Shared;
using Blueprint.API.Repository.Helper;
using Blueprint.API.Models.Shared;
using Template.Models.Dtos;
using Template.Models.Models;
using Template.Models; // for AuthMappings

namespace Blueprint.API.Repository.UserRepository
{
    /// <summary>
    /// Implements authentication-related data access using Dapper and stored procedures.
    /// </summary>
    public class AuthRepository(DatabaseSettings databaseSettings)
        : RepositoryBase(databaseSettings), IAuthRepository
    {
        /// <summary>
        /// Retrieves the stored password hash for a username.
        /// </summary>
        /// <param name="username">The username to look up.</param>
        /// <returns>An ApiResponse with the stored hash string.</returns>
        public async Task<ApiResponse<string>> GetPasswordHashByUsername(string username)
        {
            DynamicParameters p = new();
            p.Add("@Username", username, dbType: DbType.String, size: 255);
            p.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            p.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Call a dedicated sproc that returns the stored password hash for the given username
            var loginRow = await ExecuteQuery<User>("dbo.usp_GetPasswordHashByUsername", CommandType.StoredProcedure, p);
            var hash = loginRow?.UserPassword;

            string responseMessage = p.Get<string>("@ResponseMessage");
            int returnCode = p.Get<int>("@ErrorCode");

            return ApiResponseRepoHelper.HandleDatabaseResponse<string>(returnCode, responseMessage, successData: hash, successMessage: hash ?? string.Empty);
        }

        /// <summary>
        /// Retrieves user details by username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> containing user details.</returns>
        public async Task<ApiResponse<UserDetailDto>> GetUserByUsername(string username)
        {
            DynamicParameters userParameters = new();
            userParameters.Add("@Username", username, dbType: DbType.String, size: 255);
            userParameters.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            userParameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var user = await ExecuteQuery<User>("dbo.usp_GetUserSummaryByUsername", CommandType.StoredProcedure, userParameters);
            var dto = user?.ToUserDetailDto();
        
            string responseMessage = userParameters.Get<string>("@ResponseMessage");
            int returnCode = userParameters.Get<int>("@ErrorCode");

            return ApiResponseRepoHelper.HandleDatabaseResponse<UserDetailDto>(returnCode, responseMessage, successData: dto, successMessage: "User retrieved successfully.");
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userRegister">The registration data.</param>
        /// <returns>An <see cref="ApiResponse{UserDetailDto}"/> with created user details.</returns>
        public async Task<ApiResponse<UserDetailDto>> RegisterUser(User user)
        {
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

            return ApiResponseRepoHelper.HandleDatabaseResponse<UserDetailDto>(returnCode, responseMessage, successData: dto, successMessage: "User registered successfully.");
        }
    }
}
