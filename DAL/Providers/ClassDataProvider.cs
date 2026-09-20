using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;
using Shared.Entities;
using Shared.Exceptions;

namespace DAL.Providers;

public static class ClassDataProvider
{
    public static Class? GetById(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetClassById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapClass(reader);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetById(int id)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<Class?> GetByIdAsync(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetClassById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapClass(reader);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetByIdAsync(int id)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<Class> GetClassesByCourseId(int courseId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetClassesByCourseId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = courseId;

            reader = command.ExecuteReader();
            var classes = new List<Class>();
            while (reader.Read())
            {
                classes.Add(MapClass(reader));
            }
            return classes;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetClassesByCourseId(int courseId)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Class>> GetClassesByCourseIdAsync(int courseId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetClassesByCourseId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = courseId;

            reader = await command.ExecuteReaderAsync();
            var classes = new List<Class>();
            while (await reader.ReadAsync())
            {
                classes.Add(MapClass(reader));
            }
            return classes;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetClassesByCourseIdAsync()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<Class> GetAll()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetAllClasses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = command.ExecuteReader();
            var classes = new List<Class>();
            while (reader.Read())
            {
                classes.Add(MapClass(reader));
            }
            return classes;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetAll()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Class>> GetAllAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllClasses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = await command.ExecuteReaderAsync();
            var classes = new List<Class>();
            while (await reader.ReadAsync())
            {
                classes.Add(MapClass(reader));
            }
            return classes;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in Class.GetAllAsync()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static int Add(Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            command.ExecuteNonQuery();

            return (int)newIdParam.Value;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in Class.Add()", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<int> AddAsync(Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            await command.ExecuteNonQueryAsync();

            return (int)newIdParam.Value;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.AddAsync(Class entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static void Update(int id, Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.Update(int id, Class entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(int id, Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.UpdateAsync(int id, Class entity)", ex);
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

            command = new SqlCommand("usp_DeleteClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.Delete(int id)", ex);
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

            command = new SqlCommand("usp_DeleteClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in DeleteAsync(int id)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<Class> Search(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_SearchClasses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = command.ExecuteReader();
            var classes = new List<Class>();
            while (reader.Read())
            {
                classes.Add(MapClass(reader));
            }
            return classes;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.Search(string regex)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Class>> SearchAsync(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchClasses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
            var classes = new List<Class>();
            while (await reader.ReadAsync())
            {
                classes.Add(MapClass(reader));
            }
            return classes;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in SearchAsync(string regex)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    private static Class MapClass(SqlDataReader reader)
    {
        return new Class
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
            ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
            Instructor = reader.GetString(reader.GetOrdinal("Instructor")),
            MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity")),
            CurrentCapacity = reader.GetInt32(reader.GetOrdinal("CurrentCapacity")),
            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
            Schedule = (Class.DaysOfWeek)reader.GetInt32(reader.GetOrdinal("Schedule")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }
}
