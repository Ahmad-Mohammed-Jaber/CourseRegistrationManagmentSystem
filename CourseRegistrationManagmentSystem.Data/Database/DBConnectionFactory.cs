namespace CourseRegistrationManagmentSystem.Data.Database;

using Microsoft.Data.SqlClient;
public static class DBConnectionFactory
{
    private const string ConnectionString = "Data Source=.,1433;User Id=sa;Password=P@ssw0rd;Initial Catalog=CourseManagementDB;Encrypt=False;";

    public static SqlConnection CreateConnection()
    {
        return new SqlConnection(ConnectionString);
    }

}
