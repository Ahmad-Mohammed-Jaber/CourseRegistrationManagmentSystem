using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class UserRepository : IGenericRepository<User>
{
    public async Task<User?> GetByUserNameAsync(string userName)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserName, PasswordHash, FullName, Role, IsActive FROM [User] WHERE UserName = @UserName;";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = userName;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                UserName = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                Role = (User.UserRoles)reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;

    }

    public User? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, UserName, PasswordHash, FullName, Role, IsActive FROM [User] WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                UserName = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                Role = (User.UserRoles)reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserName, PasswordHash, FullName, Role, IsActive FROM [User] WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetGuid(0),
                UserName = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                Role = (User.UserRoles)reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public List<User> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, UserName, PasswordHash, FullName, Role, IsActive FROM [User]";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        var users = new List<User>();
        while (reader.Read())
        {
            users.Add(new User
            {
                Id = reader.GetGuid(0),
                UserName = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                Role = (User.UserRoles)reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return users;
    }

    public async Task<List<User>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserName, PasswordHash, FullName, Role, IsActive FROM [User]";
        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var users = new List<User>();
        while (await reader.ReadAsync())
        {
            users.Add(new User
            {
                Id = reader.GetGuid(0),
                UserName = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                FullName = reader.GetString(3),
                Role = (User.UserRoles)reader.GetInt32(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return users;
    }

    public void Add(User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "INSERT INTO [User] (Id, UserName, PasswordHash, FullName, Role, IsActive) " +
                     "VALUES (@Id, @UserName, @PasswordHash, @FullName, @Role, @IsActive)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName;
        insertCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash;
        insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        insertCommand.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        insertCommand.ExecuteNonQuery();
    }

    public async Task AddAsync(User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO [User] (Id, UserName, PasswordHash, FullName, Role, IsActive) " +
                     "VALUES (@Id, @UserName, @PasswordHash, @FullName, @Role, @IsActive)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName;
        insertCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash;
        insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        insertCommand.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        await insertCommand.ExecuteNonQueryAsync();
    }

    public void Update(Guid id, User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "UPDATE [User] SET UserName = @UserName, PasswordHash = @PasswordHash, " +
                     "FullName = @FullName, Role = @Role, IsActive = @IsActive " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName;
        updateCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash;
        updateCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        updateCommand.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
        updateCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        updateCommand.ExecuteNonQuery();
    }

    public async Task UpdateAsync(Guid id, User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "UPDATE [User] SET UserName = @UserName, PasswordHash = @PasswordHash, " +
                     "FullName = @FullName, Role = @Role, IsActive = @IsActive " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName;
        updateCommand.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash;
        updateCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        updateCommand.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
        updateCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        await updateCommand.ExecuteNonQueryAsync();
    }

    public void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "DELETE FROM [User] WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        deleteCommand.ExecuteNonQuery();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM [User] WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        await deleteCommand.ExecuteNonQueryAsync();
    }
}
