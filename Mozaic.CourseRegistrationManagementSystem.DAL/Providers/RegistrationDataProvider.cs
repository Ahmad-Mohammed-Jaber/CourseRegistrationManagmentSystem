using Microsoft.Data.SqlClient;
using System.Data;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Enums;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.DAL.Database;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Providers;

public static class RegistrationDataProvider
{
    public static async Task<RegistrationResult> GetAsync(
        int? id = null,
        int? studentId = null,
        int? classId = null,
        string? search = null,
        string? studentFilter = null,
        string? courseFilter = null,
        string? classFilter = null,
        string? statusFilter = null,
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
                connection, id, studentId, classId, search, studentFilter,
                courseFilter, classFilter, statusFilter, pageNumber, pageSize);

            reader = await command.ExecuteReaderAsync();
            var registrations = new List<Registration>();
            while (await reader.ReadAsync())
            {
                registrations.Add(MapRegistration(reader));
            }

            var result = new RegistrationResult
            {
                Items = registrations,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? registrations.Count,
                TotalCount = registrations.Count,
            };

            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                result.TotalCount = HasColumn(reader, "TotalCount") && !reader.IsDBNull(reader.GetOrdinal("TotalCount"))
                    ? reader.GetInt32(reader.GetOrdinal("TotalCount"))
                    : registrations.Count;
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetAsync(...)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<Registration?> GetByIdAsync(int id)
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

            command = CreateGetCommand(connection, id, null, null, null, null, null, null, null, null, null);

            reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapRegistration(reader) : null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetByIdAsync(int id)", ex);
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
        int? studentId,
        int? classId,
        string? search,
        string? studentFilter,
        string? courseFilter,
        string? classFilter,
        string? statusFilter,
        int? pageNumber,
        int? pageSize)
    {
        var command = new SqlCommand("usp_GetRegistrations", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id.HasValue ? id.Value : DBNull.Value;
        command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId.HasValue ? studentId.Value : DBNull.Value;
        command.Parameters.Add("@ClassId", SqlDbType.Int).Value = classId.HasValue ? classId.Value : DBNull.Value;
        command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : (object)search!;
        command.Parameters.Add("@StudentFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(studentFilter) ? DBNull.Value : (object)studentFilter!.Trim();
        command.Parameters.Add("@CourseFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(courseFilter) ? DBNull.Value : (object)courseFilter!.Trim();
        command.Parameters.Add("@ClassFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(classFilter) ? DBNull.Value : (object)classFilter!.Trim();
        command.Parameters.Add("@StatusFilter", SqlDbType.NVarChar, 20).Value =
            string.IsNullOrWhiteSpace(statusFilter) ? DBNull.Value : (object)statusFilter!.Trim();
        command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber.HasValue ? pageNumber.Value : DBNull.Value;
        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize.HasValue ? pageSize.Value : DBNull.Value;
        return command;
    }

    private static Registration MapRegistration(SqlDataReader reader)
    {
        return new Registration
        {
            Id = reader.GetInt32(reader.GetOrdinal("RegistrationId")),
            StudentId = reader.GetInt32(reader.GetOrdinal("RegistrationStudentId")),
            ClassId = reader.GetInt32(reader.GetOrdinal("RegistrationClassId")),
            RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
            Status = reader.GetString(reader.GetOrdinal("RegistrationStatus")),
            CreatedOn = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("RegistrationCreatedOn")),
            ModifiedOn = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("RegistrationModifiedOn")),
            CreatedBy = reader.GetInt32(reader.GetOrdinal("RegistrationCreatedBy")),
            ModifiedBy = reader.GetInt32(reader.GetOrdinal("RegistrationModifiedBy")),
            CreatedByName = ReadNullableString(reader, "CreatedByName"),
            ModifiedByName = ReadNullableString(reader, "ModifiedByName"),
            Class = new Class
            {
                Id = reader.GetInt32(reader.GetOrdinal("ClassId")),
                CourseId = reader.GetInt32(reader.GetOrdinal("ClassCourseId")),
                ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                Instructor = reader.GetString(reader.GetOrdinal("ClassInstructor")),
                MaxCapacity = reader.GetInt32(reader.GetOrdinal("ClassMaxCapacity")),
                CurrentCapacity = reader.GetInt32(reader.GetOrdinal("ClassCurrentCapacity")),
                StartDate = reader.GetDateTime(reader.GetOrdinal("ClassStartDate")),
                EndDate = reader.GetDateTime(reader.GetOrdinal("ClassEndDate")),
                Schedule = (DaysOfTheWeek)reader.GetInt32(reader.GetOrdinal("ClassSchedule")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("ClassIsActive")),
            },
            Student = new Student
            {
                Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                UserId = reader.GetInt32(reader.GetOrdinal("StudentUserId")),
                StudentNumber = reader.GetInt32(reader.GetOrdinal("StudentNumber")),
                Email = reader.GetString(reader.GetOrdinal("StudentEmail")),
                Phone = reader.GetString(reader.GetOrdinal("StudentPhone")),
                UserName = reader.GetString(reader.GetOrdinal("StudentUserName")),
                FullName = reader.GetString(reader.GetOrdinal("StudentFullName")),
                Role = (User.UserRoles)reader.GetInt32(reader.GetOrdinal("StudentRole")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("StudentIsActive")),
            },
            Course = new Course
            {
                Id = reader.GetInt32(reader.GetOrdinal("CourseId")),
                CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
                CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
            },
        };
    }

    private static string ReadNullableString(SqlDataReader reader, string name)
    {
        int ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
    }

    public static void Add(Registration entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = entity.StudentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = entity.ClassId;
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegistrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = command.ExecuteNonQuery();

            int? newId = newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : null;
            if (rows == 0 || newId == null || newId <= 0)
            {
                throw new DatabaseException("An error occured in RegistrationDataProvider.Add(Registration entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.Add(Registration entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task AddAsync(Registration entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = entity.StudentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = entity.ClassId;
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegistrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = entity.CreatedBy;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = await command.ExecuteNonQueryAsync();

            int? newId = newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : null;
            if (rows == 0 || newId == null || newId <= 0)
            {
                throw new DatabaseException("An error occured in RegistrationDataProvider.AddAsync(Registration entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.AddAsync(Registration entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static void Update(Registration entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = entity.StudentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = entity.ClassId;
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegistrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in RegistrationDataProvider.Update(Registration entity): Registration with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.Update(Registration entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(Registration entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = entity.StudentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = entity.ClassId;
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegistrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in RegistrationDataProvider.UpdateAsync(Registration entity): Registration with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.UpdateAsync(Registration entity)", ex);
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

            command = new SqlCommand("usp_DeleteRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in RegistrationDataProvider.Delete(int id): Registration with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.Delete(int id)", ex);
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

            command = new SqlCommand("usp_DeleteRegistration", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in RegistrationDataProvider.DeleteAsync(int id): Registration with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.DeleteAsync(int id)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
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
