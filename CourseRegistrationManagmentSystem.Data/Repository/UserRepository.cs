using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using Microsoft.Data.SqlClient;
using System.Data;

public class UserRepository : IGenericRepository<User>
{
    public void Add(User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "INSERT INTO [User] (Id, UserName, PasswordHash, FullName, Role, IsActive) " +
            "VALUES " +
            "(@Id, @UserName, @PasswordHash, @FullName, @Role, @IsActive)";

        using var insertCommand = new SqlCommand(sql, connection);

        insertCommand.CommandType = CommandType.Text;

        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName;
        insertCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash;
        insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        insertCommand.Parameters.Add("@Role", SqlDbType.Int).Value = entity.Role;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        insertCommand.ExecuteNonQuery();
    }

    public async Task AddAsync(User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO [User] (Id, UserName, PasswordHash, FullName, Role, IsActive) " +
            "VALUES " +
            "(@Id, @UserName, @PasswordHash, @FullName, @Role, @IsActive)";

        using var insertCommand = new SqlCommand(sql, connection);

        insertCommand.CommandType = CommandType.Text;

        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName;
        insertCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash;
        insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        insertCommand.Parameters.Add("@Role", SqlDbType.Int).Value = entity.Role;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
        
        await insertCommand.ExecuteNonQueryAsync();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public User? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Update(Guid id, User entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, User entity)
    {
        throw new NotImplementedException();
    }
}
