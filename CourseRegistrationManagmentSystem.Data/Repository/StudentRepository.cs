using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class StudentRepository : IGenericRepository<Student>
{
    public Student? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            };
        }
        return null;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            };
        }
        return null;
    }

    public async Task<Student?> GetByUserIdAsync(Guid userId)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student WHERE UserId = @UserId";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            };
        }
        return null;
    }

    public List<Student> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        var students = new List<Student>();
        while (reader.Read())
        {
            students.Add(new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            });
        }
        return students;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student";
        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var students = new List<Student>();
        while (await reader.ReadAsync())
        {
            students.Add(new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            });
        }
        return students;
    }

    public void Add(Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "INSERT INTO Student (Id, UserId, StudentNumber, FullName, Email, Phone) " +
                     "VALUES (@Id, @UserId, @StudentNumber, @FullName, @Email, @Phone)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = entity.UserId;
        insertCommand.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
        insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        insertCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = entity.Email;
        insertCommand.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone;

        insertCommand.ExecuteNonQuery();
    }

    public async Task AddAsync(Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO Student (Id, UserId, StudentNumber, FullName, Email, Phone) " +
                     "VALUES (@Id, @UserId, @StudentNumber, @FullName, @Email, @Phone)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = entity.UserId;
        insertCommand.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
        insertCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        insertCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = entity.Email;
        insertCommand.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone;

        await insertCommand.ExecuteNonQueryAsync();
    }

    public void Update(Guid id, Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "UPDATE Student SET UserId = @UserId, StudentNumber = @StudentNumber, " +
                     "FullName = @FullName, Email = @Email, Phone = @Phone " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = entity.UserId;
        updateCommand.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
        updateCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        updateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = entity.Email;
        updateCommand.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone;

        updateCommand.ExecuteNonQuery();
    }

    public async Task UpdateAsync(Guid id, Student entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "UPDATE Student SET UserId = @UserId, StudentNumber = @StudentNumber, " +
                     "FullName = @FullName, Email = @Email, Phone = @Phone " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = entity.UserId;
        updateCommand.Parameters.Add("@StudentNumber", SqlDbType.Int).Value = entity.StudentNumber;
        updateCommand.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = entity.FullName;
        updateCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = entity.Email;
        updateCommand.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = entity.Phone;

        await updateCommand.ExecuteNonQueryAsync();
    }

    public void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "DELETE FROM Student WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        deleteCommand.ExecuteNonQuery();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM Student WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        await deleteCommand.ExecuteNonQueryAsync();
    }

    public List<Student> Search(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student WHERE FullName LIKE @regex OR Email LIKE @regex";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@regex", SqlDbType.NVarChar).Value = $"%{regex}%";

        using var reader = command.ExecuteReader();
        var students = new List<Student>();
        while (reader.Read())
        {
            students.Add(new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            });
        }
        return students;
    }

    public async Task<List<Student>> SearchAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, UserId, StudentNumber, FullName, Email, Phone FROM Student WHERE FullName LIKE @regex OR Email LIKE @regex";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@regex", SqlDbType.NVarChar).Value = $"%{regex}%";

        using var reader = await command.ExecuteReaderAsync();
        var students = new List<Student>();
        while (await reader.ReadAsync())
        {
            students.Add(new Student
            {
                Id = reader.GetGuid(0),
                UserId = reader.GetGuid(1),
                StudentNumber = reader.GetInt32(2),
                FullName = reader.GetString(3),
                Email = reader.GetString(4),
                Phone = reader.GetString(5)
            });
        }
        return students;
    }
}
