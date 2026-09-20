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
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            command.ExecuteNonQuery();

            return (int)newIdParam.Value;
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
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
            command.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status ?? string.Empty;

            var newIdParam = new SqlParameter("@NewId", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            await command.ExecuteNonQueryAsync();

            return (int)newIdParam.Value;
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
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
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
            command.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
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
                    Id = reader.GetInt32(0),
                    StudentId = reader.GetInt32(1),
                    ClassId = reader.GetInt32(2),
                    RegsitrationDate = reader.GetDateTime(3),
                    Status = reader.GetString(4)
                };

                var cls = new Class
                {
                    Id = reader.GetInt32(5),
                    CourseId = reader.GetInt32(6),
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
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in ", ex);
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
            Id = reader.GetInt32(0),
            StudentId = reader.GetInt32(1),
            ClassId = reader.GetInt32(3),
            RegsitrationDate = reader.GetDateTime(7),
            Status = reader.GetString(8)
        };

        var student = new Student
        {
            Id = reader.GetInt32(1),
            UserName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
        };

        var cls = new Class
        {
            Id = reader.GetInt32(3),
            ClassName = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
        };

        var course = new Course
        {
            Id = reader.GetInt32(5),
            CourseName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
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
            RegsitrationDate = reader.GetDateTime(reader.GetOrdinal("RegistrationDate")),
            Status = reader.GetString(reader.GetOrdinal("Status"))
        };
    }
}
