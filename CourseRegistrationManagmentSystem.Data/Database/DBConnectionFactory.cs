using Microsoft.Data.SqlClient;

namespace DAL.Database;

public static class DBConnectionFactory
{
    private const string ConnectionString = "Server=localhost;Database=CourseManagementDB;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
