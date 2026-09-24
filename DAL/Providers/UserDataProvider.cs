using DAL.Database;
using Microsoft.Data.SqlClient;
using Shared.Entities;
using Shared.Exceptions;
using System.Data;

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
            throw new DatabaseException("An error occured in UserDataProvider.GetById(int id)", ex);
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.GetByIdAsync(int id)", ex);
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.GetByUserNameAsync(string userName)", ex);
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.GetAll()", ex);
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.GetAllAsync()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static RequestResult Add(User entity)
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
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = command.ExecuteNonQuery();

            return rows == 0 ? RequestResult.Failure($"User Not Added") : RequestResult.Success();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.Add(User entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<RequestResult> AddAsync(User entity)
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
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = await command.ExecuteNonQueryAsync();

            return rows == 0 ? RequestResult.Failure($"User Not Added") : RequestResult.Success();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.AddAsync(User entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static RequestResult Update(int id, User entity)
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
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rows = command.ExecuteNonQuery();
            return rows == 0 ? RequestResult.NotFound($"User with id {id} not found.") : RequestResult.Success(rows);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.Update(int id, User entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<RequestResult> UpdateAsync(int id, User entity)
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
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rowsAsync = await command.ExecuteNonQueryAsync();
            return rowsAsync == 0 ? RequestResult.NotFound($"User with id {id} not found.") : RequestResult.Success(rowsAsync);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.UpdateAsync(int id, User entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static RequestResult Delete(int id)
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
            int rows = command.ExecuteNonQuery();
            return rows == 0 ? RequestResult.NotFound($"User with id {id} not found.") : RequestResult.Success(rows);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.Delete(int id)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<RequestResult> DeleteAsync(int id)
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
            int rowsAsync = await command.ExecuteNonQueryAsync();
            return rowsAsync == 0 ? RequestResult.NotFound($"User with id {id} not found.") : RequestResult.Success(rowsAsync);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.DeleteAsync(int id)", ex);
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.Search(string regex)", ex);
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in UserDataProvider.SearchAsync(string regex)", ex);
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
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedOn = HasColumn(reader, "CreatedOn") && !reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedOn")) : default,
            ModifiedOn = HasColumn(reader, "ModifiedOn") && !reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ModifiedOn")) : default,
            CreatedBy = HasColumn(reader, "CreatedBy") && !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetInt32(reader.GetOrdinal("CreatedBy")) : 0,
            ModifiedBy = HasColumn(reader, "ModifiedBy") && !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? reader.GetInt32(reader.GetOrdinal("ModifiedBy")) : 0
        };
    }

    private static bool HasColumn(SqlDataReader reader, string name)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
