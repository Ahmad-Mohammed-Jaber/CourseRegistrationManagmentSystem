using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class RegistrationRepository : IGenericRepository<Registration>
{
    public Registration? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Registration
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

    public async Task<Registration?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Registration
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

    public async Task<bool> ExistsAsync(Guid studentId, Guid classId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT 1 FROM Registrations WHERE StudentId = @StudentId AND ClassId = @ClassId;";

        using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@StudentId", SqlDbType.UniqueIdentifier).Value = studentId;
        command.Parameters.Add("@ClassId", SqlDbType.UniqueIdentifier).Value = classId;

        return await command.ExecuteScalarAsync() is not null;
    }

    public List<Registration> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        var registrations = new List<Registration>();
        while (reader.Read())
        {
            registrations.Add(new Registration
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

    public async Task<List<Registration>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations";
        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var registrations = new List<Registration>();
        while (await reader.ReadAsync())
        {
            registrations.Add(new Registration
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

    public void Add(Registration entity)
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

    public async Task AddAsync(Registration entity)
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

    public void Update(Guid id, Registration entity)
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

    public async Task UpdateAsync(Guid id, Registration entity)
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

    public List<Registration> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations WHERE Status LIKE @regex";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@regex", SqlDbType.NVarChar).Value = $"%{regex}%";

        using var reader = command.ExecuteReader();
        var registrations = new List<Registration>();
        while (reader.Read())
        {
            registrations.Add(new Registration
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

    public async Task<List<Registration>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, StudentId, ClassId, RegistrationDate, Status FROM Registrations WHERE Status LIKE @regex";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@regex", SqlDbType.NVarChar).Value = $"%{regex}%";

        using var reader = await command.ExecuteReaderAsync();
        var registrations = new List<Registration>();
        while (await reader.ReadAsync())
        {
            registrations.Add(new Registration
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
}
