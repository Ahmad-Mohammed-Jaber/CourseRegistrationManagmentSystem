using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;
using Shared.Entities;

namespace DAL.Providers;

public static class CourseDataProvider
{
    public static Course? GetById(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetCourseById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapCourse(reader);
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

    public static async Task<Course?> GetByIdAsync(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetCourseById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapCourse(reader);
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

    public static List<Course> GetAll()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetAllCourses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = command.ExecuteReader();
            var courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(MapCourse(reader));
            }
            return courses;
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

    public static async Task<List<Course>> GetAllAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllCourses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = await command.ExecuteReaderAsync();
            var courses = new List<Course>();
            while (await reader.ReadAsync())
            {
                courses.Add(MapCourse(reader));
            }
            return courses;
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

    public static int Add(Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 6).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
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

    public static async Task<int> AddAsync(Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 6).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            await command.ExecuteNonQueryAsync();

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

    public static void Update(int id, Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 6).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
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

    public static async Task UpdateAsync(int id, Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 6).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
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

            command = new SqlCommand("usp_DeleteCourse", connection)
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

            command = new SqlCommand("usp_DeleteCourse", connection)
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

    public static List<Course> Search(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_SearchCourses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = command.ExecuteReader();
            var courses = new List<Course>();
            while (reader.Read())
            {
                courses.Add(MapCourse(reader));
            }
            return courses;
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

    public static async Task<List<Course>> SearchAsync(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchCourses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
            var courses = new List<Course>();
            while (await reader.ReadAsync())
            {
                courses.Add(MapCourse(reader));
            }
            return courses;
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

    private static Course MapCourse(SqlDataReader reader)
    {
        return new Course
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
            CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
            CreditHours = (double)reader.GetDecimal(reader.GetOrdinal("CreditHours")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }
}