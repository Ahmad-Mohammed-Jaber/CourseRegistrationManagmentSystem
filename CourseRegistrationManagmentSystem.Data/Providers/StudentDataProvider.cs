using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Database;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Providers;

public static class StudentDataProvider
{
    public static Student? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetStudentById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapStudent(reader);
        }
        return null;
    }

    public static async Task<Student?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetStudentById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapStudent(reader);
        }
        return null;
    }

    public static async Task<Student?> GetByUserIdAsync(Guid userId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetStudentByUserId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapStudent(reader);
        }
        return null;
    }

    public static List<Student> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetAllStudents", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = command.ExecuteReader();
        var students = new List<Student>();
        while (reader.Read())
        {
            students.Add(MapStudent(reader));
        }
        return students;
    }

    public static async Task<List<Student>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetAllStudents", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = await command.ExecuteReaderAsync();
        var students = new List<Student>();
        while (await reader.ReadAsync())
        {
            students.Add(MapStudent(reader));
        }
        return students;
    }

    public static void Add(Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_CreateStudent", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddStudentParameters(command, entity);
        command.ExecuteNonQuery();
    }

    public static async Task AddAsync(Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_CreateStudent", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddStudentParameters(command, entity);
        await command.ExecuteNonQueryAsync();
    }

    public static void Update(Guid id, Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_UpdateStudent", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddStudentParameters(command, entity, id);
        command.ExecuteNonQuery();
    }

    public static async Task UpdateAsync(Guid id, Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_UpdateStudent", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddStudentParameters(command, entity, id);
        await command.ExecuteNonQueryAsync();
    }

    public static void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_DeleteStudent", connection)
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

        using var command = new SqlCommand("usp_DeleteStudent", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await command.ExecuteNonQueryAsync();
    }

    public static List<Student> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_SearchStudents", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = command.ExecuteReader();
        var students = new List<Student>();
        while (reader.Read())
        {
            students.Add(MapStudent(reader));
        }
        return students;
    }

    public static async Task<List<Student>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_SearchStudents", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();
        var students = new List<Student>();
        while (await reader.ReadAsync())
        {
            students.Add(MapStudent(reader));
        }
        return students;
    }

    private static Student MapStudent(SqlDataReader reader)
    {
        return new Student
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
            StudentNumber = reader.GetInt32(reader.GetOrdinal("StudentNumber")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Phone = reader.GetString(reader.GetOrdinal("Phone"))
        };
    }

    private static void AddStudentParameters(SqlCommand command, Student entity, Guid? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = entity.UserId;
        command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = (object?)entity.FullName ?? DBNull.Value;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = entity.Email ?? string.Empty;
        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone ?? string.Empty;
    }
}
