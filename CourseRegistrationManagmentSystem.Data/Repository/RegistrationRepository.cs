using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class RegistrationRepository : IGenericRepository<Registrations>
{
    public Registrations? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Registrations
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                ClassId = reader.GetGuid(2),
                RegsitrationDate = reader.GetDateTime(3),
                Status = reader.GetString(4)
            };
        }
        return null;
    }

    public async Task<Registrations?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Registrations
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                ClassId = reader.GetGuid(2),
                RegsitrationDate = reader.GetDateTime(3),
                Status = reader.GetString(4)
            };
        }
        return null;
    }

    public List<Registrations> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        var registrations = new List<Registrations>();
        while (reader.Read())
        {
            registrations.Add(new Registrations
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                ClassId = reader.GetGuid(2),
                RegsitrationDate = reader.GetDateTime(3),
                Status = reader.GetString(4)
            });
        }
        return registrations;
    }

    public async Task<List<Registrations>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations";
        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var registrations = new List<Registrations>();
        while (await reader.ReadAsync())
        {
            registrations.Add(new Registrations
            {
                Id = reader.GetGuid(0),
                StudentId = reader.GetGuid(1),
                ClassId = reader.GetGuid(2),
                RegsitrationDate = reader.GetDateTime(3),
                Status = reader.GetString(4)
            });
        }
        return registrations;
    }

    public void Add(Registrations entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "INSERT INTO Registrations (Id, StudentId, ClassId, RegistrationDate, Status) " +
                     "VALUES (@Id, @StudentId, @ClassId, @RegistrationDate, @Status)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = entity.StudentId;
        insertCommand.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = entity.ClassId;
        insertCommand.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
        insertCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status;

        insertCommand.ExecuteNonQuery();
    }

    public async Task AddAsync(Registrations entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO Registrations (Id, StudentId, ClassId, RegistrationDate, Status) " +
                     "VALUES (@Id, @StudentId, @ClassId, @RegistrationDate, @Status)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = entity.StudentId;
        insertCommand.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = entity.ClassId;
        insertCommand.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
        insertCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status;

        await insertCommand.ExecuteNonQueryAsync();
    }

    public void Update(Guid id, Registrations entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "UPDATE Registrations SET StudentId = @StudentId, ClassId = @ClassId, " +
                     "RegistrationDate = @RegistrationDate, Status = @Status " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = entity.StudentId;
        updateCommand.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = entity.ClassId;
        updateCommand.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
        updateCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status;

        updateCommand.ExecuteNonQuery();
    }

    public async Task UpdateAsync(Guid id, Registrations entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "UPDATE Registrations SET StudentId = @StudentId, ClassId = @ClassId, " +
                     "RegistrationDate = @RegistrationDate, Status = @Status " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = entity.StudentId;
        updateCommand.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = entity.ClassId;
        updateCommand.Parameters.Add("@RegistrationDate", SqlDbType.DateTime2).Value = entity.RegsitrationDate;
        updateCommand.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = entity.Status;

        await updateCommand.ExecuteNonQueryAsync();
    }

    public void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "DELETE FROM Registrations WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        deleteCommand.ExecuteNonQuery();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM Registrations WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        await deleteCommand.ExecuteNonQueryAsync();
    }
}
