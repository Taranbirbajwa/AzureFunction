using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GNPMAzureFunctions.Services
{
    public class DatabaseService: IDatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DynamicFormDB");
        }
        public  async Task ExecuteStoredProcedureAsync(Func<SqlCommand, Task> action, string procedureName, SqlParameter[]? parameters = null)
        {
            //var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DynamicFormDB");
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.CommandText = procedureName;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    await action(command);
                }
            }
        }
    }
}
