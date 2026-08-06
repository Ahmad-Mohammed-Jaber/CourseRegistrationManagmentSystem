using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using CourseRegistrationManagmentSystem.Data.Database;

namespace CourseRegistrationManagmentSystem.Data.Providers;

public static class ClassDataProvider
{
    public static Class? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetClassById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapClass(reader);
        }
        return null;
    }

    public static async Task<Class?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetClassById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapClass(reader);
        }
        return null;
    }

    public static List<Class> GetClassesByCourseId(Guid courseId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetClassesByCourseId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = courseId;

        using var reader = command.ExecuteReader();
        var classes = new List<Class>();
        while (reader.Read())
        {
            classes.Add(MapClass(reader));
        }
        return classes;
    }

    public static async Task<List<Class>> GetClassesByCourseIdAsync(Guid courseId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetClassesByCourseId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = courseId;

        using var reader = await command.ExecuteReaderAsync();
        var classes = new List<Class>();
        while (await reader.ReadAsync())
        {
            classes.Add(MapClass(reader));
        }
        return classes;
    }

    public static List<Class> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetAllClasses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = command.ExecuteReader();
        var classes = new List<Class>();
        while (reader.Read())
        {
            classes.Add(MapClass(reader));
        }
        return classes;
    }

    public static async Task<List<Class>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetAllClasses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = await command.ExecuteReaderAsync();
        var classes = new List<Class>();
        while (await reader.ReadAsync())
        {
            classes.Add(MapClass(reader));
        }
        return classes;
    }

    public static void Add(Class entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_CreateClass", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddClassParameters(command, entity);
        command.ExecuteNonQuery();
    }

    public static async Task AddAsync(Class entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_CreateClass", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddClassParameters(command, entity);
        await command.ExecuteNonQueryAsync();
    }

    public static void Update(Guid id, Class entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_UpdateClass", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddClassParameters(command, entity, id);
        command.ExecuteNonQuery();
    }

    public static async Task UpdateAsync(Guid id, Class entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_UpdateClass", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddClassParameters(command, entity, id);
        await command.ExecuteNonQueryAsync();
    }

    public static void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_DeleteClass", connection)
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

        using var command = new SqlCommand("usp_DeleteClass", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await command.ExecuteNonQueryAsync();
    }

    public static List<Class> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_SearchClasses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = command.ExecuteReader();
        var classes = new List<Class>();
        while (reader.Read())
        {
            classes.Add(MapClass(reader));
        }
        return classes;
    }

    public static async Task<List<Class>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_SearchClasses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();
        var classes = new List<Class>();
        while (await reader.ReadAsync())
        {
            classes.Add(MapClass(reader));
        }
        return classes;
    }

    private static Class MapClass(SqlDataReader reader)
    {
        return new Class
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            CourseId = reader.GetGuid(reader.GetOrdinal("CourseId")),
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

    private static void AddClassParameters(SqlCommand command, Class entity, Guid? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = entity.CourseId;
        command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
        command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
        command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
        command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
        command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
        command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
        command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
    }
}
