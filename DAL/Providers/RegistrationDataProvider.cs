using Microsoft.Data.SqlClient;
using System.Data;
using DAL.Database;
using Shared.Entities;
using Shared.Exceptions;

namespace DAL.Providers;

public static class RegistrationDataProvider
{
    public static Registration? GetById(int id)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetRegistrationById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = command.ExecuteReader();
            if (reader.Read())
            {
                return MapRegistration(reader);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetById(int id)", ex);
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
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetRegistrationById", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapRegistration(reader);
            }
            return null;
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

    public static async Task<bool> ExistsAsync(int studentId, int classId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_RegistrationExists", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = classId;

            return await command.ExecuteScalarAsync() is not null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.ExistsAsync(int studentId, int classId)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<Registration> GetRegistrationsByStudentId(int studentId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetRegistrationsByStudentId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

            reader = command.ExecuteReader();
            var list = new List<Registration>();
            while (reader.Read())
            {
                list.Add(MapRegistration(reader));
            }
            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetRegistrationsByStudentId(int studentId)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Registration>> GetRegistrationsByStudentIdAsync(int studentId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetRegistrationsByStudentId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

            reader = await command.ExecuteReaderAsync();
            var list = new List<Registration>();
            while (await reader.ReadAsync())
            {
                list.Add(MapRegistration(reader));
            }
            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetRegistrationsByStudentIdAsync(int studentId)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<Registration> GetRegistrationsByClassId(int classId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetRegistrationsByClassId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = classId;

            reader = command.ExecuteReader();
            var list = new List<Registration>();
            while (reader.Read())
            {
                list.Add(MapRegistration(reader));
            }
            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetRegistrationsByClassId(int classId)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Registration>> GetRegistrationsByClassIdAsync(int classId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetRegistrationsByClassId", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = classId;

            reader = await command.ExecuteReaderAsync();
            var list = new List<Registration>();
            while (await reader.ReadAsync())
            {
                list.Add(MapRegistration(reader));
            }
            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetRegistrationsByClassIdAsync(int classId)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static List<Registration> GetAll()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_GetAllRegistrations", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = command.ExecuteReader();
            var registrations = new List<Registration>();
            while (reader.Read())
            {
                registrations.Add(MapRegistration(reader));
            }
            return registrations;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetAll()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Registration>> GetAllAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllRegistrations", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = await command.ExecuteReaderAsync();
            var registrations = new List<Registration>();
            while (await reader.ReadAsync())
            {
                registrations.Add(MapRegistration(reader));
            }
            return registrations;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetAllAsync()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static int Add(Registration entity)
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

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            command.ExecuteNonQuery();

            entity.Id = (int)newIdParam.Value;
            return entity.Id;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetAllAsync()", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<int> AddAsync(Registration entity)
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

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            await command.ExecuteNonQueryAsync();

            entity.Id = (int)newIdParam.Value;
            return entity.Id;
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

    public static void Update(int id, Registration entity)
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
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = entity.StudentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = entity.ClassId;
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegistrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;

            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.Update(int id, Registration entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task UpdateAsync(int id, Registration entity)
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
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = entity.StudentId;
            command.Parameters.Add("@ClassId", SqlDbType.Int).Value = entity.ClassId;
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegistrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.UpdateAsync(int id, Registration entity)", ex);
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
            command.ExecuteNonQuery();
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
            await command.ExecuteNonQueryAsync();
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

    public static List<Registration> Search(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            connection.Open();

            command = new SqlCommand("usp_SearchRegistrations", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = command.ExecuteReader();
            var registrations = new List<Registration>();
            while (reader.Read())
            {
                registrations.Add(MapRegistration(reader));
            }
            return registrations;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.Search(string regex)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<Registration>> SearchAsync(string regex)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchRegistrations", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = regex ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
            var registrations = new List<Registration>();
            while (await reader.ReadAsync())
            {
                registrations.Add(MapRegistration(reader));
            }
            return registrations;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.SearchAsync(string regex)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<(Registration Registration, Class Class)>> GetStudentRegistrationsWithClassesAsync(int studentId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetStudentRegistrationsWithClasses", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;

            reader = await command.ExecuteReaderAsync();

            var list = new List<(Registration Registration, Class Class)>();

            while (await reader.ReadAsync())
            {
                var registration = new Registration
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                    ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
                    RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
                    Status = reader.GetString(reader.GetOrdinal("Status"))
                };

                var cls = new Class
                {
                    // SP aliases Class PK as Class_Id to avoid collision with Registration Id (r.Id).
                    Id = reader.GetInt32(reader.GetOrdinal("Class_Id")),
                    CourseId = reader.GetInt32(reader.GetOrdinal("CourseId")),
                    ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
                    Instructor = reader.GetString(reader.GetOrdinal("Instructor")),
                    MaxCapacity = reader.GetInt32(reader.GetOrdinal("MaxCapacity")),
                    CurrentCapacity = reader.GetInt32(reader.GetOrdinal("CurrentCapacity")),
                    StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                    EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                    Schedule = (Class.DaysOfWeek)reader.GetInt32(reader.GetOrdinal("Schedule")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                };

                list.Add((registration, cls));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured while GetStudentRegistrationsWithClassesAsync()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> GetAllDetailedAsync()
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetAllRegistrationsDetailed", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            reader = await command.ExecuteReaderAsync();

            var list = new List<(Registration, Student, Class, Course)>();

            while (await reader.ReadAsync())
            {
                list.Add(MapDetailedRow(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.GetAllDetailedAsync()", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> SearchDetailedAsync(string searchTerm)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_SearchRegistrationsDetailed", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@regex", SqlDbType.NVarChar, 100).Value = searchTerm ?? string.Empty;

            reader = await command.ExecuteReaderAsync();

            var list = new List<(Registration, Student, Class, Course)>();

            while (await reader.ReadAsync())
            {
                list.Add(MapDetailedRow(reader));
            }

            return list;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RegistrationDataProvider.SearchDetailedAsync(string searchTerm)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    private static (Registration, Student, Class, Course) MapDetailedRow(SqlDataReader reader)
    {
        var registration = new Registration
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
            ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
            RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            CreatedOn = HasColumn(reader, "CreatedOn") && !reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedOn")) : default,
            ModifiedOn = HasColumn(reader, "ModifiedOn") && !reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ModifiedOn")) : default,
            CreatedBy = HasColumn(reader, "CreatedBy") && !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetInt32(reader.GetOrdinal("CreatedBy")) : 0,
            ModifiedBy = HasColumn(reader, "ModifiedBy") && !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? reader.GetInt32(reader.GetOrdinal("ModifiedBy")) : 0
        };

        var student = new Student
        {
            Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserName"))
        };

        var cls = new Class
        {
            Id = reader.GetInt32(reader.GetOrdinal("ClassId")),
            ClassName = reader.IsDBNull(reader.GetOrdinal("ClassName")) ? string.Empty : reader.GetString(reader.GetOrdinal("ClassName"))
        };

        var course = new Course
        {
            Id = reader.GetInt32(reader.GetOrdinal("CourseId")),
            CourseName = reader.IsDBNull(reader.GetOrdinal("CourseName")) ? string.Empty : reader.GetString(reader.GetOrdinal("CourseName"))
        };

        return (registration, student, cls, course);
    }

    private static Registration MapRegistration(SqlDataReader reader)
    {
        return new Registration
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
            ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
            RegistrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            CreatedOn = HasColumn(reader, "CreatedOn") && !reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedOn")) : default,
            ModifiedOn = HasColumn(reader, "ModifiedOn") && !reader.IsDBNull(reader.GetOrdinal("ModifiedOn")) ? reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ModifiedOn")) : default,
            CreatedBy = HasColumn(reader, "CreatedBy") && !reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? reader.GetInt32(reader.GetOrdinal("CreatedBy")) : 0,
            ModifiedBy = HasColumn(reader, "ModifiedBy") && !reader.IsDBNull(reader.GetOrdinal("ModifiedBy")) ? reader.GetInt32(reader.GetOrdinal("ModifiedBy")) : 0
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
}
