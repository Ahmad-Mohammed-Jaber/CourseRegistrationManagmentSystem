using Microsoft.Data.SqlClient;
using System.Data;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.DAL.Database;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Providers;

public static class StudentDataProvider
{
    public static async Task<StudentResult> GetAsync(
        int? id = null,
        int? userId = null,
        string? search = null,
        string? studentNumberFilter = null,
        string? fullNameFilter = null,
        string? userNameFilter = null,
        string? emailFilter = null,
        string? phoneFilter = null,
        bool? isActive = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = CreateGetCommand(
                connection, id, userId, search, studentNumberFilter, fullNameFilter,
                userNameFilter, emailFilter, phoneFilter, isActive, pageNumber, pageSize);

            reader = await command.ExecuteReaderAsync();
            var students = new List<Student>();
            while (await reader.ReadAsync())
            {
                students.Add(MapStudent(reader));
            }

            var result = new StudentResult
            {
                Items = students,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? students.Count,
                TotalCount = students.Count,
            };

            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                result.TotalCount = HasColumn(reader, "TotalCount") && !reader.IsDBNull(reader.GetOrdinal("TotalCount"))
                    ? reader.GetInt32(reader.GetOrdinal("TotalCount"))
                    : students.Count;
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in StudentDataProvider.GetAsync(...)", ex);
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
        if (id <= 0)
        {
            return null;
        }

        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = CreateGetCommand(connection, id, null, null, null, null, null, null, null, null, null, null);

            reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapStudent(reader) : null;
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

    private static SqlCommand CreateGetCommand(
        SqlConnection connection,
        int? id,
        int? userId,
        string? search,
        string? studentNumberFilter,
        string? fullNameFilter,
        string? userNameFilter,
        string? emailFilter,
        string? phoneFilter,
        bool? isActive,
        int? pageNumber,
        int? pageSize)
    {
        var command = new SqlCommand("usp_GetStudents", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id.HasValue ? id.Value : DBNull.Value;
        command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId.HasValue ? userId.Value : DBNull.Value;
        command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : (object)search!;
        command.Parameters.Add("@StudentNumberFilter", SqlDbType.NVarChar, 30).Value =
            string.IsNullOrWhiteSpace(studentNumberFilter) ? DBNull.Value : (object)studentNumberFilter!.Trim();
        command.Parameters.Add("@FullNameFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(fullNameFilter) ? DBNull.Value : (object)fullNameFilter!.Trim();
        command.Parameters.Add("@UserNameFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(userNameFilter) ? DBNull.Value : (object)userNameFilter!.Trim();
        command.Parameters.Add("@EmailFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(emailFilter) ? DBNull.Value : (object)emailFilter!.Trim();
        command.Parameters.Add("@PhoneFilter", SqlDbType.NVarChar, 20).Value =
            string.IsNullOrWhiteSpace(phoneFilter) ? DBNull.Value : (object)phoneFilter!.Trim();
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive.HasValue ? isActive.Value : DBNull.Value;
        command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber.HasValue ? pageNumber.Value : DBNull.Value;
        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize.HasValue ? pageSize.Value : DBNull.Value;
        return command;
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
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy;

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
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy;

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
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

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
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

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

        if (HasColumn(reader, "PasswordHash") && !reader.IsDBNull(reader.GetOrdinal("PasswordHash")))
        {
            student.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
        }

        student.CreatedByName = ReadNullableString(reader, "CreatedByName");
        student.ModifiedByName = ReadNullableString(reader, "ModifiedByName");

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

    private static string ReadNullableString(SqlDataReader reader, string name)
    {
        int ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }
}
