using Microsoft.Data.SqlClient;
using System.Data;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Enums;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.DAL.Database;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Providers;

public static class ClassDataProvider
{
    public static async Task<ClassResult> GetAsync(
        int? id = null,
        int? courseId = null,
        int? userId = null,
        string? search = null,
        string? statusFilter = null,
        string? classNameFilter = null,
        string? instructorFilter = null,
        string? courseFilter = null,
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
                connection, id, courseId, userId, search, statusFilter,
                classNameFilter, instructorFilter, courseFilter, pageNumber, pageSize);

            reader = await command.ExecuteReaderAsync();
            var classes = new List<Class>();
            while (await reader.ReadAsync())
            {
                classes.Add(MapClass(reader));
            }

            var result = new ClassResult
            {
                Items = classes,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? classes.Count,
                TotalCount = classes.Count,
            };

            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                result.TotalCount = ReadInt32(reader, "TotalCount", classes.Count);
                result.OpenSeatsCount = ReadInt32(reader, "OpenSeatsCount", 0);
                result.MyRegistrationsCount = ReadInt32(reader, "MyRegistrationsCount", 0);
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetAsync(...)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<Class?> GetByIdAsync(int id)
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
            return await reader.ReadAsync() ? MapClass(reader) : null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.GetByIdAsync(int id)", ex);
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
        int? courseId,
        int? userId,
        string? search,
        string? statusFilter,
        string? classNameFilter,
        string? instructorFilter,
        string? courseFilter,
        int? pageNumber,
        int? pageSize)
    {
        var command = new SqlCommand("usp_GetClasses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id.HasValue ? id.Value : DBNull.Value;
        command.Parameters.Add("@CourseId", SqlDbType.Int).Value = courseId.HasValue ? courseId.Value : DBNull.Value;
        command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId.HasValue ? userId.Value : DBNull.Value;
        command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : (object)search!;
        command.Parameters.Add("@StatusFilter", SqlDbType.NVarChar, 10).Value =
            string.IsNullOrWhiteSpace(statusFilter) ? DBNull.Value : (object)statusFilter!;
        command.Parameters.Add("@ClassNameFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(classNameFilter) ? DBNull.Value : (object)classNameFilter!.Trim();
        command.Parameters.Add("@InstructorFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(instructorFilter) ? DBNull.Value : (object)instructorFilter!.Trim();
        command.Parameters.Add("@CourseFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(courseFilter) ? DBNull.Value : (object)courseFilter!.Trim();
        command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber.HasValue ? pageNumber.Value : DBNull.Value;
        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize.HasValue ? pageSize.Value : DBNull.Value;
        return command;
    }

    private static int ReadInt32(SqlDataReader reader, string column, int fallback)
    {
        return HasColumn(reader, column) && !reader.IsDBNull(reader.GetOrdinal(column))
            ? reader.GetInt32(reader.GetOrdinal(column))
            : fallback;
    }

    public static void Add(Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
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
                throw new DatabaseException("An error occured in ClassDataProvider.Add(Class entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.Add(Class entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task AddAsync(Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
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
                throw new DatabaseException("An error occured in ClassDataProvider.AddAsync(Class entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.AddAsync(Class entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static void Update(Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in ClassDataProvider.Update(Class entity): Class with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.Update(Class entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(Class entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@CourseId", SqlDbType.Int).Value = entity.CourseId;
            command.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName ?? string.Empty;
            command.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor ?? string.Empty;
            command.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
            command.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
            command.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
            command.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
            command.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in ClassDataProvider.UpdateAsync(Class entity): Class with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.UpdateAsync(Class entity)", ex);
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

            command = new SqlCommand("usp_DeleteClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in ClassDataProvider.Delete(int id): Class with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.Delete(int id)", ex);
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

            command = new SqlCommand("usp_DeleteClass", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in ClassDataProvider.DeleteAsync(int id): Class with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ClassDataProvider.DeleteAsync(int id)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    private static Class MapClass(SqlDataReader reader)
    {
        var entity = new Class
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
            ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
            Instructor = reader.GetString(reader.GetOrdinal("Instructor")),
            MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity")),
            CurrentCapacity = reader.GetInt32(reader.GetOrdinal("CurrentCapacity")),
            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
            // usp_GetClasses returns the raw schedule int as Schedule; the
            // legacy browse procedure aliased it as ScheduleDisplay.
            Schedule = HasColumn(reader, "Schedule") && !reader.IsDBNull(reader.GetOrdinal("Schedule"))
                ? (DaysOfTheWeek)reader.GetInt32(reader.GetOrdinal("Schedule"))
                : HasColumn(reader, "ScheduleDisplay") && !reader.IsDBNull(reader.GetOrdinal("ScheduleDisplay"))
                    ? (DaysOfTheWeek)reader.GetInt32(reader.GetOrdinal("ScheduleDisplay"))
                    : default,
            IsActive = HasColumn(reader, "IsActive") && !reader.IsDBNull(reader.GetOrdinal("IsActive"))
                ? reader.GetBoolean(reader.GetOrdinal("IsActive"))
                : true,
            CreatedOn = HasColumn(reader, "CreatedOn") && !reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedOn")) : default,
            ModifiedOn = HasColumn(reader, "ModifiedOn") && !reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ModifiedOn")) : default,
            CreatedBy = HasColumn(reader, "CreatedBy") && !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetInt32(reader.GetOrdinal("CreatedBy")) : 0,
            ModifiedBy = HasColumn(reader, "ModifiedBy") && !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? reader.GetInt32(reader.GetOrdinal("ModifiedBy")) : 0,
            CreatedByName = ReadNullableString(reader, "CreatedByName"),
            ModifiedByName = ReadNullableString(reader, "ModifiedByName")
        };

        if (HasColumn(reader, "CourseName") && !reader.IsDBNull(reader.GetOrdinal("CourseName")))
        {
            entity.Course = new Course
            {
                Id = entity.CourseId,
                CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
                CourseCode = HasColumn(reader, "CourseCode") && !reader.IsDBNull(reader.GetOrdinal("CourseCode"))
                    ? reader.GetString(reader.GetOrdinal("CourseCode"))
                    : string.Empty
            };
        }

        if (HasColumn(reader, "IsRegistered") && !reader.IsDBNull(reader.GetOrdinal("IsRegistered")))
        {
            entity.IsRegistered = reader.GetBoolean(reader.GetOrdinal("IsRegistered"));
        }

        return entity;
    }

    private static string ReadNullableString(SqlDataReader reader, string name)
    {
        int ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
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
