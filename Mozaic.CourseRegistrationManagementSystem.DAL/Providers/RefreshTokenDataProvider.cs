using Microsoft.Data.SqlClient;
using Mozaic.CourseRegistrationManagementSystem.DAL.Database;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using System.Data;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Providers;

public static class RefreshTokenDataProvider
{
    public static async Task AddAsync(RefreshToken entity)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_CreateRefreshToken", connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            command.Parameters.Add("@UserId", SqlDbType.Int).Value = entity.UserId;
            command.Parameters.Add("@TokenHash", SqlDbType.Char, 64).Value = entity.TokenHash;
            command.Parameters.Add("@ExpiresAt", SqlDbType.DateTimeOffset).Value = entity.ExpiresAt;

            var newIdParam = new SqlParameter("@Id", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            command.Parameters.Add(newIdParam);

            int rows = await command.ExecuteNonQueryAsync();

            int? newId = newIdParam.Value != DBNull.Value ? (int)newIdParam.Value : null;
            if (rows == 0 || newId == null || newId <= 0)
            {
                throw new DatabaseException("An error occured in RefreshTokenDataProvider.AddAsync(RefreshToken entity): database reported no new id.");
            }

            entity.Id = newId.Value;
        }
        catch (DatabaseException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RefreshTokenDataProvider.AddAsync(RefreshToken entity)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        SqlDataReader? reader = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_GetRefreshTokenByHash", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@TokenHash", SqlDbType.Char, 64).Value = tokenHash ?? string.Empty;

            reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapRefreshToken(reader);
            }
            return null;
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RefreshTokenDataProvider.GetByHashAsync(string tokenHash)", ex);
        }
        finally
        {
            reader?.Dispose();
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task RevokeAsync(string tokenHash)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_RevokeRefreshToken", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@TokenHash", SqlDbType.Char, 64).Value = tokenHash ?? string.Empty;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RefreshTokenDataProvider.RevokeAsync(string tokenHash)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task RevokeAllForUserAsync(int userId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_RevokeRefreshTokensForUser", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RefreshTokenDataProvider.RevokeAllForUserAsync(int userId)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    private static RefreshToken MapRefreshToken(SqlDataReader reader)
    {
        var revokedOrdinal = reader.GetOrdinal("RevokedAt");
        return new RefreshToken
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            TokenHash = reader.GetString(reader.GetOrdinal("TokenHash")),
            ExpiresAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ExpiresAt")),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedAt")),
            RevokedAt = reader.IsDBNull(revokedOrdinal) ? null : reader.GetFieldValue<DateTimeOffset>(revokedOrdinal),
        };
    }
}
