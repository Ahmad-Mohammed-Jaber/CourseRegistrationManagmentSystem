using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;

namespace DAL.Providers;

public static class CourseDataProvider
{
    public static Course? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetCourseById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapCourse(reader);
        }
        return null;
    }

    public static async Task<Course?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetCourseById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapCourse(reader);
        }
        return null;
    }

    public static List<Course> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetAllCourses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = command.ExecuteReader();
        var courses = new List<Course>();
        while (reader.Read())
        {
            courses.Add(MapCourse(reader));
        }
        return courses;
    }

    public static async Task<List<Course>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetAllCourses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = await command.ExecuteReaderAsync();
        var courses = new List<Course>();
        while (await reader.ReadAsync())
        {
            courses.Add(MapCourse(reader));
        }
        return courses;
    }

    public static void Add(Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_CreateCourse", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddCourseParameters(command, entity);
        command.ExecuteNonQuery();
    }

    public static async Task AddAsync(Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_CreateCourse", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddCourseParameters(command, entity);
        await command.ExecuteNonQueryAsync();
    }

    public static void Update(Guid id, Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_UpdateCourse", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddCourseParameters(command, entity, id);
        command.ExecuteNonQuery();
    }

    public static async Task UpdateAsync(Guid id, Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_UpdateCourse", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddCourseParameters(command, entity, id);
        await command.ExecuteNonQueryAsync();
    }

    public static void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_DeleteCourse", connection)
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

        using var command = new SqlCommand("usp_DeleteCourse", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await command.ExecuteNonQueryAsync();
    }

    public static List<Course> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_SearchCourses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = command.ExecuteReader();
        var courses = new List<Course>();
        while (reader.Read())
        {
            courses.Add(MapCourse(reader));
        }
        return courses;
    }

    public static async Task<List<Course>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_SearchCourses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();
        var courses = new List<Course>();
        while (await reader.ReadAsync())
        {
            courses.Add(MapCourse(reader));
        }
        return courses;
    }

    private static Course MapCourse(SqlDataReader reader)
    {
        return new Course
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
            CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
            CreditHours = (double)reader.GetDecimal(reader.GetOrdinal("CreditHours")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
        };
    }

    private static void AddCourseParameters(SqlCommand command, Course entity, Guid? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 6).Value = entity.CourseCode ?? string.Empty;
        command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
        command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
        command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
    }
}
