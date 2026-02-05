using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Blueprint.API.Models.Shared; // updated to use DatabaseSettings from App.API.Models

namespace Blueprint.API.Repository.Shared
{
    /// <summary>
    /// Base repository providing common Dapper-based data access helpers.
    /// </summary>
    public abstract class RepositoryBase
    {
        private readonly DatabaseSettings _databaseSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        /// <param name="databaseSettings">Database settings containing the connection string.</param>
        protected RepositoryBase(DatabaseSettings databaseSettings)
        {
            _databaseSettings = databaseSettings;
        }

        /// <summary>
        /// Executes a query and maps the results to the specified type.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="sqlCommand">The SQL or stored procedure name.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="parameters">Optional parameters.</param>
        /// <returns>A sequence of <typeparamref name="T"/>.</returns>
        protected async Task<IEnumerable<T>> ExecuteQueryList<T>(string sqlCommand, CommandType commandType, DynamicParameters? parameters = null)
        {
            using (var connection = new SqlConnection(_databaseSettings.AppDbConnectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<T>(sqlCommand, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Executes a query and maps the results to the specified type.
        /// </summary>
        /// <typeparam name="T">Result type.</typeparam>
        /// <param name="sqlCommand">The SQL or stored procedure name.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="parameters">Optional parameters.</param>
        /// <returns>The first result of <typeparamref name="T"/> or default.</returns>
        protected async Task<T?> ExecuteQuery<T>(string sqlCommand, CommandType commandType, DynamicParameters? parameters = null)
        {
            using (var connection = new SqlConnection(_databaseSettings.AppDbConnectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstOrDefaultAsync<T>(sqlCommand, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Executes a non-query command.
        /// </summary>
        /// <param name="sqlCommand">The SQL or stored procedure name.</param>
        /// <param name="commandType">Command type.</param>
        /// <param name="parameters">Optional parameters.</param>
        /// <returns>The number of rows affected.</returns>
        protected async Task<int> ExecuteCommand(string sqlCommand, CommandType commandType, DynamicParameters? parameters = null)
        {
            using (var connection = new SqlConnection(_databaseSettings.AppDbConnectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(sqlCommand, parameters, commandType: commandType);
            }
        }

        /// <summary>
        /// Creates a new SQL connection for advanced scenarios.
        /// </summary>
        /// <returns>An open <see cref="SqlConnection"/>.</returns>
        public SqlConnection GetConnection()
        {
            return new SqlConnection(_databaseSettings.AppDbConnectionString);
        }
    }
}
