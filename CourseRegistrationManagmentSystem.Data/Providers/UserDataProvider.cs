using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;
using Shared.Entities;

namespace DAL.Providers;

public static class UserDataProvider
{
    public static User? GetById(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetUserById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapUser(reader);
            }
            return null;
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task<User?> GetByIdAsync(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetUserById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUser(reader);
            }
            return null;
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task<User?> GetByUserNameAsync(string userName)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetUserByUserName", connection)
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
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static List<User> GetAll()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetAllUsers", connection)
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
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task<List<User>> GetAllAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllUsers", connection)
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
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static void Add(User entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddUserParameters(command, entity);
            command.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task AddAsync(User entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddUserParameters(command, entity);
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static void Update(int id, User entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddUserParameters(command, entity, id);
            command.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task UpdateAsync(int id, User entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddUserParameters(command, entity, id);
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static void Delete(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_DeleteUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task DeleteAsync(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_DeleteUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static List<User> Search(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_SearchUsers", connection)
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
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    public static async Task<List<User>> SearchAsync(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchUsers", connection)
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
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Close();
        }
    }

    private static User MapUser(SqlDataReader reader)
    {
        return new User
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserName = reader.GetString(reader.GetOrdinal("UserName")),
            PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Role = (User.UserRoles)reader.GetInt32(reader.GetOrdinal("Role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }

    private static void AddUserParameters(SqlCommand command, User entity, int? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.Int).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName ?? string.Empty;
        command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash ?? string.Empty;
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName ?? string.Empty;
        command.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
    }
}