using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;
using Shared.Entities;
using Shared.Exceptions;
using Shared.DTOs;

namespace DAL.Providers;

public static class StudentDataProvider
{
    public static StudentProfile? GetProfileByUserId(int userId)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_GetStudentProfileByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapStudentProfile(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetProfileByUserId(int userId)", ex);
        }
    }

    public static async Task<StudentProfile?> GetProfileByUserIdAsync(int userId)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_GetStudentProfileByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudentProfile(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetProfileByUserIdAsync(int userId)", ex);
        }
    }

    private static StudentProfile MapStudentProfile(SqlDataReader reader)
    {
        object roleVal = reader.GetValue(reader.GetOrdinal("Role"));
        string roleStr = roleVal is int i ? ((User.UserRoles)i).ToString() : roleVal?.ToString() ?? string.Empty;

        // Canonical columns: StudentId + UserId. Fall back to legacy "Id" (StudentId).
        int studentId = HasColumn(reader, "StudentId") && !reader.IsDBNull(reader.GetOrdinal("StudentId"))
            ? reader.GetInt32(reader.GetOrdinal("StudentId"))
            : reader.GetInt32(reader.GetOrdinal("Id"));
        int userId = HasColumn(reader, "UserId") && !reader.IsDBNull(reader.GetOrdinal("UserId"))
            ? reader.GetInt32(reader.GetOrdinal("UserId"))
            : studentId;

        return new StudentProfile(
            studentId,
            userId,
            reader.GetString(reader.GetOrdinal("UserName")),
            reader.GetString(reader.GetOrdinal("FullName")),
            roleStr,
            reader.IsDBNull(reader.GetOrdinal("StudentNumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("StudentNumber")),
            reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
            reader.IsDBNull(reader.GetOrdinal("Phone")) ? string.Empty : reader.GetString(reader.GetOrdinal("Phone"))
        );
    }


    public static Student? GetById(int id)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_GetStudentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapStudent(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetById(int id)", ex);
        }
    }

    public static async Task<Student?> GetByIdAsync(int id)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_GetStudentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudent(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetByIdAsync(int id)", ex);
        }
    }

    public static Student? GetByUserId(int userId)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_GetStudentByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapStudent(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetByUserId(int userId)", ex);
        }
    }

    public static async Task<Student?> GetByUserIdAsync(int userId)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_GetStudentByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudent(reader);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetByUserIdAsync(int userId)", ex);
        }
    }

    public static List<Student> GetAll()
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_GetAllStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            using var reader = command.ExecuteReader();
            var list = new List<Student>();
            while (reader.Read())
            {
                list.Add(MapStudent(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetAll()", ex);
        }
    }

    public static async Task<List<Student>> GetAllAsync()
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_GetAllStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            using var reader = await command.ExecuteReaderAsync();
            var list = new List<Student>();
            while (await reader.ReadAsync())
            {
                list.Add(MapStudent(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetAllAsync()", ex);
        }
    }

    public static int Add(Student entity)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_CreateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            var outId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(outId);

            command.ExecuteNonQuery();
            entity.Id = (int)outId.Value;
            return entity.Id;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Add(Student entity)", ex);
        }
    }

    public static async Task<int> AddAsync(Student entity)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_CreateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            var outId = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            command.Parameters.Add(outId);

            await command.ExecuteNonQueryAsync();
            if (outId.Value is int id && id != 0)
            {
                entity.Id = id;
            }
            return entity.Id;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.AddAsync(Student entity)", ex);
        }
    }

    public static void Update(int id, Student entity)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_UpdateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Update(int id, Student entity)", ex);
        }
    }

    public static async Task UpdateAsync(int id, Student entity)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_UpdateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.UpdateAsync(int id, Student entity)", ex);
        }
    }

    public static void Delete(int id)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_DeleteStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Delete(int id)", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_DeleteStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.DeleteAsync(int id)", ex);
        }
    }

    public static List<Student> Search(string regex)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            connection.Open();
            using var command = new SqlCommand("usp_SearchStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            using var reader = command.ExecuteReader();
            var list = new List<Student>();
            while (reader.Read())
            {
                list.Add(MapStudent(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Search(string regex)", ex);
        }
    }

    public static async Task<List<Student>> SearchAsync(string regex)
    {
        try
        {
            using var connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();
            using var command = new SqlCommand("usp_SearchStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            using var reader = await command.ExecuteReaderAsync();
            var list = new List<Student>();
            while (await reader.ReadAsync())
            {
                list.Add(MapStudent(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.SearchAsync(string regex)", ex);
        }
    }

    private static Student MapStudent(SqlDataReader reader)
    {
        var student = new Student
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            StudentNumber = reader.IsDBNull(reader.GetOrdinal("StudentNumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("StudentNumber")),
            Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
            Phone = reader.IsDBNull(reader.GetOrdinal("Phone")) ? string.Empty : reader.GetString(reader.GetOrdinal("Phone"))
        };

        if (HasColumn(reader, "UserName") && !reader.IsDBNull(reader.GetOrdinal("UserName")))
        {
            student.UserName = reader.GetString(reader.GetOrdinal("UserName"));
        }

        if (HasColumn(reader, "FullName") && !reader.IsDBNull(reader.GetOrdinal("FullName")))
        {
            student.FullName = reader.GetString(reader.GetOrdinal("FullName"));
        }

        if (HasColumn(reader, "IsActive") && !reader.IsDBNull(reader.GetOrdinal("IsActive")))
        {
            student.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
        }

        if (HasColumn(reader, "Role") && !reader.IsDBNull(reader.GetOrdinal("Role")))
        {
            student.Role = (User.UserRoles)reader.GetInt32(reader.GetOrdinal("Role"));
        }

        if (HasColumn(reader, "CreatedOn") && !reader.IsDBNull(reader.GetOrdinal("CreatedOn")))
        {
            student.CreatedOn = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedOn"));
        }

        if (HasColumn(reader, "ModifiedOn") && !reader.IsDBNull(reader.GetOrdinal("ModifiedOn")))
        {
            student.ModifiedOn = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ModifiedOn"));
        }

        if (HasColumn(reader, "CreatedBy") && !reader.IsDBNull(reader.GetOrdinal("CreatedBy")))
        {
            student.CreatedBy = reader.GetInt32(reader.GetOrdinal("CreatedBy"));
        }

        if (HasColumn(reader, "ModifiedBy") && !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")))
        {
            student.ModifiedBy = reader.GetInt32(reader.GetOrdinal("ModifiedBy"));
        }

        return student;
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
