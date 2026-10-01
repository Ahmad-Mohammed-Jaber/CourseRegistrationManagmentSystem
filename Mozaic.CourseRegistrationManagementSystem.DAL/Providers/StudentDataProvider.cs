using Microsoft.Data.SqlClient;
using System.Data;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.DAL.Database;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Providers;

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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetById(int id)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetByIdAsync(int id)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static Student? GetByUserId(int userId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetStudentByUserId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            reader = command.ExecuteReader();
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
        finally
        {
            reader?.Dispose();
            command?.Dispose();
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetByUserIdAsync(int userId)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
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
        finally
        {
            reader?.Dispose();
            command?.Dispose();
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
        finally
        {
            reader?.Dispose();
            command?.Dispose();
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
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = command.ExecuteNonQuery();

            int? newId = newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : null;
            if (rows == 0 || newId == null || newId <= 0)
            {
                throw new DatabaseException("An error occured in StudentDataProvider.Add(Student entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Add(Student entity)", ex);
        }
        finally
        {
            command?.Dispose();
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
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = await command.ExecuteNonQueryAsync();

            int? newId = newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : null;
            if (rows == 0 || newId == null || newId <= 0)
            {
                throw new DatabaseException("An error occured in StudentDataProvider.AddAsync(Student entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.AddAsync(Student entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static void Update(Student entity)
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
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in StudentDataProvider.Update(Student entity): Student with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Update(Student entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(Student entity)
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
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
            command.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = (object?)entity.Email ?? string.Empty;
            command.Parameters.Add("@Phone", SqlDbType.NVarChar, 20).Value = (object?)entity.Phone ?? string.Empty;

            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in StudentDataProvider.UpdateAsync(Student entity): Student with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.UpdateAsync(Student entity)", ex);
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

            command = new SqlCommand("usp_DeleteStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in StudentDataProvider.Delete(int id): Student with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.Delete(int id)", ex);
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

            command = new SqlCommand("usp_DeleteStudent", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in StudentDataProvider.DeleteAsync(int id): Student with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.DeleteAsync(int id)", ex);
        }
        finally
        {
            command?.Dispose();
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
        finally
        {
            reader?.Dispose();
            command?.Dispose();
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
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
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
