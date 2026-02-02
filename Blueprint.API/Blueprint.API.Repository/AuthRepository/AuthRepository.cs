using Dapper;
using System.Data;
using Blueprint.API.Repository.Shared;
using Blueprint.API.Repository.Helper;
using Blueprint.API.Models.Shared;
using Template.Models.Dtos;
using Template.Models.Models;

namespace Blueprint.API.Repository.UserRepository
{
    public class AuthRepository(DatabaseSettings databaseSettings)
        : RepositoryBase(databaseSettings), IAuthRepository
    {
        public async Task<ApiResponse<IEnumerable<AuthDetailsDto>>> GetUserByUsername(string username)
        {
            DynamicParameters userParameters = new();
            userParameters.Add("@Username", username, dbType: DbType.String, size: 255);
            userParameters.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            userParameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var userDetail = await ExecuteQuery<AuthDetailsDto>("dbo.usp_GetUserSummaryByUsername", CommandType.StoredProcedure, userParameters);

            string responseMessage = userParameters.Get<string>("@ResponseMessage");
            int returnCode = userParameters.Get<int>("@ErrorCode");

            return ApiResponseRepoHelper.HandleDatabaseResponse(returnCode, responseMessage, successData: userDetail, successMessage: "User retrieved successfully.");
        }

        public async Task<ApiResponse<UserLoginResponseDto>> LoginUser(string username)
        {
            DynamicParameters userLoginParameters = new();
            userLoginParameters.Add("@Username", username, dbType: DbType.String, size: 255);
            userLoginParameters.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            userLoginParameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var userDetail = (await ExecuteQuery<UserLoginResponseDto>("dbo.usp_GetUserForLogin", CommandType.StoredProcedure, userLoginParameters)).FirstOrDefault();

            string responseMessage = userLoginParameters.Get<string>("@ResponseMessage");
            int returnCode = userLoginParameters.Get<int>("@ErrorCode");

            return ApiResponseRepoHelper.HandleDatabaseResponse(returnCode, responseMessage, successData: userDetail, successMessage: "User retrieved successfully.");
        }

        public async Task<ApiResponse<AuthDetailsDto>> RegisterUser(RegisterUserDto userRegister)
        {
            DynamicParameters userRegisterParameters = new();
            userRegisterParameters.Add("@Username", userRegister.Username, dbType: DbType.String, size: 100);
            userRegisterParameters.Add("@UserPassword", userRegister.UserPassword, dbType: DbType.String, size: 255);
            userRegisterParameters.Add("@Role", userRegister.Role, dbType: DbType.String, size: 10);
            userRegisterParameters.Add("@ResponseMessage", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);
            userRegisterParameters.Add("@ErrorCode", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var userDetail = (await ExecuteQuery<AuthDetailsDto>("dbo.usp_RegisterUser", CommandType.StoredProcedure, userRegisterParameters)).FirstOrDefault();

            string responseMessage = userRegisterParameters.Get<string>("@ResponseMessage");
            int returnCode = userRegisterParameters.Get<int>("@ErrorCode");

            return ApiResponseRepoHelper.HandleDatabaseResponse(returnCode, responseMessage, successData: userDetail, successMessage: "User registered successfully.");
        }
    }
}
