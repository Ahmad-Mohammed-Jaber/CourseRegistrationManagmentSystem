using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;

namespace DAL.Providers;

public static class RegistrationDataProvider
{
    public static Registration? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetRegistrationById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapRegistration(reader);
        }
        return null;
    }

    public static async Task<Registration?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetRegistrationById", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return MapRegistration(reader);
        }
        return null;
    }

    public static async Task<bool> ExistsAsync(Guid studentId, Guid classId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_RegistrationExists", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = studentId;
        command.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = classId;

        return await command.ExecuteScalarAsync() is not null;
    }

    public static List<Registration> GetRegistrationsByStudentId(Guid studentId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetRegistrationsByStudentId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = studentId;

        using var reader = command.ExecuteReader();
        var list = new List<Registration>();
        while (reader.Read())
        {
            list.Add(MapRegistration(reader));
        }
        return list;
    }

    public static async Task<List<Registration>> GetRegistrationsByStudentIdAsync(Guid studentId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetRegistrationsByStudentId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = studentId;

        using var reader = await command.ExecuteReaderAsync();
        var list = new List<Registration>();
        while (await reader.ReadAsync())
        {
            list.Add(MapRegistration(reader));
        }
        return list;
    }

    public static List<Registration> GetRegistrationsByClassId(Guid classId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetRegistrationsByClassId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = classId;

        using var reader = command.ExecuteReader();
        var list = new List<Registration>();
        while (reader.Read())
        {
            list.Add(MapRegistration(reader));
        }
        return list;
    }

    public static async Task<List<Registration>> GetRegistrationsByClassIdAsync(Guid classId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetRegistrationsByClassId", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = classId;

        using var reader = await command.ExecuteReaderAsync();
        var list = new List<Registration>();
        while (await reader.ReadAsync())
        {
            list.Add(MapRegistration(reader));
        }
        return list;
    }

    public static List<Registration> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_GetAllRegistrations", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = command.ExecuteReader();
        var registrations = new List<Registration>();
        while (reader.Read())
        {
            registrations.Add(MapRegistration(reader));
        }
        return registrations;
    }

    public static async Task<List<Registration>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetAllRegistrations", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = await command.ExecuteReaderAsync();
        var registrations = new List<Registration>();
        while (await reader.ReadAsync())
        {
            registrations.Add(MapRegistration(reader));
        }
        return registrations;
    }

    public static void Add(Registration entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_CreateRegistration", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddRegistrationParameters(command, entity);
        command.ExecuteNonQuery();
    }

    public static async Task AddAsync(Registration entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_CreateRegistration", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddRegistrationParameters(command, entity);
        await command.ExecuteNonQueryAsync();
    }

    public static void Update(Guid id, Registration entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_UpdateRegistration", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddRegistrationParameters(command, entity, id);
        command.ExecuteNonQuery();
    }

    public static async Task UpdateAsync(Guid id, Registration entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_UpdateRegistration", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        AddRegistrationParameters(command, entity, id);
        await command.ExecuteNonQueryAsync();
    }

    public static void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_DeleteRegistration", connection)
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

        using var command = new SqlCommand("usp_DeleteRegistration", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        await command.ExecuteNonQueryAsync();
    }

    public static List<Registration> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        using var command = new SqlCommand("usp_SearchRegistrations", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = command.ExecuteReader();
        var registrations = new List<Registration>();
        while (reader.Read())
        {
            registrations.Add(MapRegistration(reader));
        }
        return registrations;
    }

    public static async Task<List<Registration>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_SearchRegistrations", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();
        var registrations = new List<Registration>();
        while (await reader.ReadAsync())
        {
            registrations.Add(MapRegistration(reader));
        }
        return registrations;
    }

    public static async Task<List<(Registration Registration, Class Class)>> GetStudentRegistrationsWithClassesAsync(Guid studentId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetStudentRegistrationsWithClasses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = studentId;

        using var reader = await command.ExecuteReaderAsync();

        var list = new List<(Registration Registration, Class Class)>();

        while (await reader.ReadAsync())
        {
            var registration = new Registration
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                ClassId = reader.GetGuid(2),
                RegsitrationDate = reader.GetDateTime(3),
                Status = reader.GetString(4)
            };

            var cls = new Class
            {
                Id = reader.GetGuid(5),
                CourseId = reader.GetGuid(6),
                ClassName = reader.GetString(7),
                Instructor = reader.GetString(8),
                MaxCapacity = reader.GetInt32(9),
                CurrentCapacity = reader.GetInt32(10),
                StartDate = reader.GetDateTime(11),
                EndDate = reader.GetDateTime(12),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(13),
                IsActive = reader.GetBoolean(14)
            };

            list.Add((registration, cls));
        }

        return list;
    }

    public static async Task<List<RegistrationDto>> GetAllDetailedAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_GetAllRegistrationsDetailed", connection)
        {
            CommandType = CommandType.StoredProcedure
        };

        using var reader = await command.ExecuteReaderAsync();

        var list = new List<RegistrationDto>();

        while (await reader.ReadAsync())
        {
            list.Add(new RegistrationDto
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                StudentUserName = reader.GetString(2),
                ClassId = reader.GetGuid(3),
                ClassName = reader.GetString(4),
                CourseName = reader.GetString(5),
                RegistrationDate = reader.GetDateTime(6),
                Status = reader.GetString(7)
            });
        }

        return list;
    }

    public static async Task<List<RegistrationDto>> SearchDetailedAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        using var command = new SqlCommand("usp_SearchRegistrationsDetailed", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

        using var reader = await command.ExecuteReaderAsync();

        var list = new List<RegistrationDto>();

        while (await reader.ReadAsync())
        {
            list.Add(new RegistrationDto
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                StudentUserName = reader.GetString(2),
                ClassId = reader.GetGuid(3),
                ClassName = reader.GetString(4),
                CourseName = reader.GetString(5),
                RegistrationDate = reader.GetDateTime(6),
                Status = reader.GetString(7)
            });
        }

        return list;
    }

    private static Registration MapRegistration(SqlDataReader reader)
    {
        return new Registration
        {
            Id = reader.GetGuid(reader.GetOrdinal("Id")),
            StudentId = reader.GetGuid(reader.GetOrdinal("StudentId")),
            ClassId = reader.GetGuid(reader.GetOrdinal("ClassId")),
            RegsitrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
            Status = reader.GetString(reader.GetOrdinal("Status"))
        };
    }

    private static void AddRegistrationParameters(SqlCommand command, Registration entity, Guid? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = entity.StudentId;
        command.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = entity.ClassId;
        command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
        command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;
    }
}
