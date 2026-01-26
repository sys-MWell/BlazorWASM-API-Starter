using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using App.API.Models.Shared; // updated to use DatabaseSettings from App.API.Models

namespace App.API.Repository.Shared
{
    public abstract class RepositoryBase
    {
        private readonly DatabaseSettings _databaseSettings;

        protected RepositoryBase(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        protected async Task<IEnumerable<T>> ExecuteQuery<T>(string sqlCommand, CommandType commandType, DynamicParameters? parameters = null)
        {
            using (var connection = new SqlConnection(_databaseSettings.AppDbConnectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sqlCommand, parameters, commandType: commandType);
            }
        }

        protected async Task<int> ExecuteCommand(string sqlCommand, CommandType commandType, DynamicParameters? parameters = null)
        {
            using (var connection = new SqlConnection(_databaseSettings.AppDbConnectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(sqlCommand, parameters, commandType: commandType);
            }
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_databaseSettings.AppDbConnectionString);
        }
    }
}
