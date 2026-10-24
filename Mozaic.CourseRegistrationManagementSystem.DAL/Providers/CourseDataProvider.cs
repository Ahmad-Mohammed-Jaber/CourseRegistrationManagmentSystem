using Microsoft.Data.SqlClient;
using System.Data;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.DAL.Database;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Providers;

public static class CourseDataProvider
{
    public static async Task<CourseResult> GetAsync(
        int? id = null,
        string? courseCode = null,
        string? search = null,
        string? codeFilter = null,
        string? nameFilter = null,
        string? descriptionFilter = null,
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
                connection, id, courseCode, search, codeFilter, nameFilter,
                descriptionFilter, isActive, pageNumber, pageSize);

            reader = await command.ExecuteReaderAsync();
            var courses = new List<Course>();
            while (await reader.ReadAsync())
            {
                courses.Add(MapCourse(reader));
            }

            var result = new CourseResult
            {
                Items = courses,
                PageNumber = pageNumber ?? 1,
                PageSize = pageSize ?? courses.Count,
                TotalCount = courses.Count,
            };

            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                result.TotalCount = HasColumn(reader, "TotalCount") && !reader.IsDBNull(reader.GetOrdinal("TotalCount"))
                    ? reader.GetInt32(reader.GetOrdinal("TotalCount"))
                    : courses.Count;
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.GetAsync(...)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<Course?> GetByIdAsync(int id)
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

            command = CreateGetCommand(connection, id);

            reader = await command.ExecuteReaderAsync();
            return await reader.ReadAsync() ? MapCourse(reader) : null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.GetByIdAsync(int id)", ex);
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
        int? id = null,
        string? courseCode = null,
        string? search = null,
        string? codeFilter = null,
        string? nameFilter = null,
        string? descriptionFilter = null,
        bool? isActive = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        var command = new SqlCommand("usp_GetCourses", connection)
        {
            CommandType = CommandType.StoredProcedure
        };
        command.Parameters.Add("@Id", SqlDbType.Int).Value = id.HasValue ? id.Value : DBNull.Value;
        command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 20).Value =
            string.IsNullOrWhiteSpace(courseCode) ? DBNull.Value : (object)courseCode!;
        command.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : (object)search!;
        command.Parameters.Add("@CodeFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(codeFilter) ? DBNull.Value : (object)codeFilter!.Trim();
        command.Parameters.Add("@NameFilter", SqlDbType.NVarChar, 100).Value =
            string.IsNullOrWhiteSpace(nameFilter) ? DBNull.Value : (object)nameFilter!.Trim();
        command.Parameters.Add("@DescriptionFilter", SqlDbType.NVarChar, 200).Value =
            string.IsNullOrWhiteSpace(descriptionFilter) ? DBNull.Value : (object)descriptionFilter!.Trim();
        command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive.HasValue ? isActive.Value : DBNull.Value;
        command.Parameters.Add("@PageNumber", SqlDbType.Int).Value = pageNumber.HasValue ? pageNumber.Value : DBNull.Value;
        command.Parameters.Add("@PageSize", SqlDbType.Int).Value = pageSize.HasValue ? pageSize.Value : DBNull.Value;
        return command;
    }

    public static void Add(Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_CreateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 20).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
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
                throw new DatabaseException("An error occured in CourseDataProvider.Add(Course entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.Add(Course entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task AddAsync(Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 20).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
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
                throw new DatabaseException("An error occured in CourseDataProvider.AddAsync(Course entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.AddAsync(Course entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static void Update(Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_UpdateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 20).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar,-1).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in CourseDataProvider.Update(Course entity): Course with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.Update(Course entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(Course entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_UpdateCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = entity.Id;
            command.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 20).Value = entity.CourseCode ?? string.Empty;
            command.Parameters.Add("@CourseName", SqlDbType.NVarChar, 100).Value = entity.CourseName ?? string.Empty;
            command.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = (decimal)entity.CreditHours;
            command.Parameters.Add("@Description", SqlDbType.NVarChar, -1).Value = entity.Description ?? string.Empty;
            command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;
            command.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = entity.ModifiedBy;

            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in CourseDataProvider.UpdateAsync(Course entity): Course with id {entity.Id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.UpdateAsync(Course entity)", ex);
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

            command = new SqlCommand("usp_DeleteCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rows = command.ExecuteNonQuery();
            if (rows == 0)
            {
                throw new DatabaseException($"An error occured in CourseDataProvider.Delete(int id): Course with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.Delete(int id)", ex);
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

            command = new SqlCommand("usp_DeleteCourse", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            int rowsAsync = await command.ExecuteNonQueryAsync();
            if (rowsAsync == 0)
            {
                throw new DatabaseException($"An error occured in CourseDataProvider.DeleteAsync(int id): Course with id {id} not found.");
            }
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in CourseDataProvider.DeleteAsync(int id)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    private static Course MapCourse(SqlDataReader reader)
    {
        return new Course
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            CourseCode = reader.GetString(reader.GetOrdinal("CourseCode")),
            CourseName = reader.GetString(reader.GetOrdinal("CourseName")),
            CreditHours = (double)reader.GetDecimal(reader.GetOrdinal("CreditHours")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
            CreatedOn = HasColumn(reader, "CreatedOn") && !reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedOn")) : default,
            ModifiedOn = HasColumn(reader, "ModifiedOn") && !reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ModifiedOn")) : default,
            CreatedBy = HasColumn(reader, "CreatedBy") && !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetInt32(reader.GetOrdinal("CreatedBy")) : 0,
            ModifiedBy = HasColumn(reader, "ModifiedBy") && !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? reader.GetInt32(reader.GetOrdinal("ModifiedBy")) : 0,
            CreatedByName = ReadNullableString(reader, "CreatedByName"),
            ModifiedByName = ReadNullableString(reader, "ModifiedByName")
        };
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
