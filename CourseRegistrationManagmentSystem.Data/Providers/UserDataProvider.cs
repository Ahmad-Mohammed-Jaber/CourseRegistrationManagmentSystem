using CourseRegistrationManagmentSystem.Data.Database;
using Microsoft.Data.SqlClient;
using System.Data;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Data.Providers;
public static class UserDataProvider
{
    public static User? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetUserById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapUser(reader);
        }
        return null;
    }

    public static async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetUserById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }
        return null;
    }

    public static async Task<User?> GetByUserNameAsync(string userName)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetUserByUserName", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = userName ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapUser(reader);
        }
        return null;
    }

    public static List<User> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetAllUsers", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = command.ExecuteReader();
        var users = new List<User>();
        while (reader.Read())
        {
            users.Add(MapUser(reader));
        }
        return users;
    }

    public static async Task<List<User>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetAllUsers", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = await command.ExecuteReaderAsync();
        var users = new List<User>();
        while (await reader.ReadAsync())
        {
            users.Add(MapUser(reader));
        }
        return users;
    }

    public static void Add(User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_CreateUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddUserParameters(command, entity);
        command.ExecuteNonQuery();
    }

    public static async Task AddAsync(User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_CreateUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddUserParameters(command, entity);
        await command.ExecuteNonQueryAsync();
    }

    public static void Update(Guid id, User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_UpdateUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddUserParameters(command, entity, id);
        command.ExecuteNonQuery();
    }

    public static async Task UpdateAsync(Guid id, User entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_UpdateUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddUserParameters(command, entity, id);
        await command.ExecuteNonQueryAsync();
    }

    public static void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_DeleteUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        command.ExecuteNonQuery();
    }

    public static async Task DeleteAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_DeleteUser", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await command.ExecuteNonQueryAsync();
    }

    public static List<User> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_SearchUsers", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = command.ExecuteReader();
        var users = new List<User>();
        while (reader.Read())
        {
            users.Add(MapUser(reader));
        }
        return users;
    }

    public static async Task<List<User>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_SearchUsers", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();
        var users = new List<User>();
        while (await reader.ReadAsync())
        {
            users.Add(MapUser(reader));
        }
        return users;
    }

    private static User MapUser(SqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            UserName = reader.GetString(reader.GetOrdinal("UserName")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Role = (User.UserRoles)reader.GetInt32(reader.GetOrdinal("Role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }

    private static void AddUserParameters(SqlCommand command, User entity, Guid? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName ?? string.Empty;
        command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash ?? string.Empty;
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName ?? string.Empty;
        command.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
    }
}
