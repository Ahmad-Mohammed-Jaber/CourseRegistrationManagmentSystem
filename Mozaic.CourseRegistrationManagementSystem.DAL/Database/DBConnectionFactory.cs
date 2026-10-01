using Microsoft.Data.SqlClient;
using Shared.Database;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Database;

public static class DBConnectionFactory
{
    public static string ConnectionString => DbConfig.ConnectionString;

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
