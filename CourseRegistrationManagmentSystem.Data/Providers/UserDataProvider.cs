using DAL.Database;
using Microsoft.Data.SqlClient;
using Shared.Entities;
using Shared.Exceptions;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DAL.Providers;

public static class UserDataProvider
{
    public static User? GetById(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetUserById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapUser(reader);
            }
            return null;
        }
        catch (Exception ex) 
        {
            throw new DatabaseException("An error occured while GetUserById(int id)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<User?> GetByIdAsync(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetUserById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = await command.ExecuteReaderAsync();
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
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<User?> GetByUserNameAsync(string userName)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetUserByUserName", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = userName ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapUser(reader);
            }
            return null;
        }
        catch
        {
            throw new DatabaseEx;
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<User> GetAll()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetAllUsers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = command.ExecuteReader();
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
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<User>> GetAllAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllUsers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = await command.ExecuteReaderAsync();
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
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static int Add(User entity)
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

            command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName ?? string.Empty;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash ?? string.Empty;
            command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName ?? string.Empty;
            command.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            command.ExecuteNonQuery();

            return (int)newIdParam.Value;
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
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

            command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName ?? string.Empty;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash ?? string.Empty;
            command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName ?? string.Empty;
            command.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            await command.ExecuteNonQueryAsync();

            //return (int) result!;
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
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
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName ?? string.Empty;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash ?? string.Empty;
            command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName ?? string.Empty;
            command.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            command.ExecuteNonQuery();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
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
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@UserName", SqlDbType.NVarChar, 100).Value = entity.UserName ?? string.Empty;
            command.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255).Value = entity.PasswordHash ?? string.Empty;
            command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName ?? string.Empty;
            command.Parameters.Add("@Role", SqlDbType.Int).Value = (int)entity.Role;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            await command.ExecuteNonQueryAsync();
        }
        catch
        {
            throw;
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
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
            connection?.Dispose();
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
            connection?.Dispose();
        }
    }

    public static List<User> Search(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_SearchUsers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = command.ExecuteReader();
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
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<User>> SearchAsync(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchUsers", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
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
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
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
}