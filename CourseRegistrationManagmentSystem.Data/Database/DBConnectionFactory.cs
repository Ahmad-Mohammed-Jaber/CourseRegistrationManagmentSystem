using Microsoft.Data.SqlClient;

namespace DAL.Database;

public static class DBConnectionFactory
{
    private const string ConnectionString = "Server=Ahmad;Database=CourseManagementDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
