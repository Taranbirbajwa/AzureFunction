using System;
using Microsoft.Data.SqlClient;

namespace GNPMAzureFunctions.Services
{
    public interface IDatabaseService
    {
        Task ExecuteStoredProcedureAsync(Func<SqlCommand, Task> action, string procedureName, SqlParameter[]? parameters = null);
    }
}
