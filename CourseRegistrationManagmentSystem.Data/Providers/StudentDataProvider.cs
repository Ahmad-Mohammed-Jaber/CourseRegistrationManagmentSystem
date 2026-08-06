using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;
using Shared.Entities;

namespace DAL.Providers;

public static class StudentDataProvider
{
    public static Student? GetById(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetStudentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapStudent(reader);
            }
            return null;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    public static async Task<Student?> GetByIdAsync(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetStudentById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudent(reader);
            }
            return null;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    public static async Task<Student?> GetByUserIdAsync(int userId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetStudentByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapStudent(reader);
            }
            return null;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    public static List<Student> GetAll()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetAllStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = command.ExecuteReader();
            var students = new List<Student>();
            while (reader.Read())
            {
                students.Add(MapStudent(reader));
            }
            return students;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    public static async Task<List<Student>> GetAllAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = await command.ExecuteReaderAsync();
            var students = new List<Student>();
            while (await reader.ReadAsync())
            {
                students.Add(MapStudent(reader));
            }
            return students;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    public static void Add(Student entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddStudentParameters(command, entity);
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
            connection?.Dispose();
        }
    }

    public static async Task AddAsync(Student entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddStudentParameters(command, entity);
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
            connection?.Dispose();
        }
    }

    public static void Update(int id, Student entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddStudentParameters(command, entity, id);
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
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(int id, Student entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            AddStudentParameters(command, entity, id);
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

            command = new SqlCommand("usp_DeleteStudent", connection)
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

            command = new SqlCommand("usp_DeleteStudent", connection)
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
            connection?.Dispose();
        }
    }

    public static List<Student> Search(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_SearchStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = command.ExecuteReader();
            var students = new List<Student>();
            while (reader.Read())
            {
                students.Add(MapStudent(reader));
            }
            return students;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    public static async Task<List<Student>> SearchAsync(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchStudents", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
            var students = new List<Student>();
            while (await reader.ReadAsync())
            {
                students.Add(MapStudent(reader));
            }
            return students;
        }
        catch
        {
            throw;
        }
        finally
        {
            reader?.Close();
            reader?.Dispose();
            command?.Dispose();
            connection?.Close();
            connection?.Dispose();
        }
    }

    private static Student MapStudent(SqlDataReader reader)
    {
        return new Student
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            UserName = reader.GetString(reader.GetOrdinal("UserName")),
            FullName = reader.GetString(reader.GetOrdinal("FullName")),
            Role = (User.UserRoles)reader.GetInt32(reader.GetOrdinal("Role")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            StudentNumber = reader.GetInt32(reader.GetOrdinal("StudentNumber")),
            Email = reader.GetString(reader.GetOrdinal("Email")),
            Phone = reader.GetString(reader.GetOrdinal("Phone"))
        };
    }

    private static void AddStudentParameters(SqlCommand command, Student entity, int? explicitId = null)
    {
        command.Parameters.Add("@Id", SqlDbType.Int).Value = explicitId ?? entity.Id;
        command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
        command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = (object?)entity.FullName ?? DBNull.Value;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = entity.Email ?? string.Empty;
        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone ?? string.Empty;
    }
}