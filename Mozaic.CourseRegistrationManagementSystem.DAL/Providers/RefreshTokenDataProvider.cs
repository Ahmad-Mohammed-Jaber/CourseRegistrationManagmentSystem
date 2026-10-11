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
            command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = entity.FamilyId;
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

    public static async Task MarkRotatedAsync(int id, string replacedByHash)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_MarkRefreshTokenRotated", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            command.Parameters.Add("@ReplacedByHash", SqlDbType.Char, 64).Value = replacedByHash;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RefreshTokenDataProvider.MarkRotatedAsync(int id, string replacedByHash)", ex);
        }
        finally
        {
            command?.Dispose();
            connection?.Dispose();
        }
    }

    public static async Task RevokeFamilyAsync(Guid familyId)
    {
        SqlConnection? connection = null;
        SqlCommand? command = null;
        try
        {
            connection = DBConnectionFactory.CreateConnection();
            await connection.OpenAsync();

            command = new SqlCommand("usp_RevokeRefreshTokenFamily", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.Add("@FamilyId", SqlDbType.UniqueIdentifier).Value = familyId;

            await command.ExecuteNonQueryAsync();
        }
        catch (Exception ex)
        {
            throw new DatabaseException("An error occured in RefreshTokenDataProvider.RevokeFamilyAsync(Guid familyId)", ex);
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
        var replacedOrdinal = reader.GetOrdinal("ReplacedByHash");
        return new RefreshToken
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
            TokenHash = reader.GetString(reader.GetOrdinal("TokenHash")),
            FamilyId = reader.GetGuid(reader.GetOrdinal("FamilyId")),
            ExpiresAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("ExpiresAt")),
            CreatedAt = reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("CreatedAt")),
            RevokedAt = reader.IsDBNull(revokedOrdinal) ? null : reader.GetFieldValue<DateTimeOffset>(revokedOrdinal),
            ReplacedByHash = reader.IsDBNull(replacedOrdinal) ? null : reader.GetString(replacedOrdinal),
        };
    }
}
