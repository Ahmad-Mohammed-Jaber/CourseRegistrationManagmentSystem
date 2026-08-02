using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class CourseRepository : IGenericRepository<Course>
{
    public Course? GetById(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive FROM Course WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Course
            {
                Id = reader.GetGuid(0),
                CourseCode = reader.GetString(1),
                CourseName = reader.GetString(2),
                CreditHours = (double)reader.GetDecimal(3),
                Description = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive FROM Course WHERE Id = @Id";
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Course
            {
                Id = reader.GetGuid(0),
                CourseCode = reader.GetString(1),
                CourseName = reader.GetString(2),
                CreditHours = (double)reader.GetDecimal(3),
                Description = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            };
        }
        return null;
    }

    public List<Course> GetAll()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive FROM Course";
        using var command = new SqlCommand(sql, connection);
        using var reader = command.ExecuteReader();

        var courses = new List<Course>();
        while (reader.Read())
        {
            courses.Add(new Course
            {
                Id = reader.GetGuid(0),
                CourseCode = reader.GetString(1),
                CourseName = reader.GetString(2),
                CreditHours = (double)reader.GetDecimal(3),
                Description = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return courses;
    }

    public async Task<List<Course>> GetAllAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT Id, CourseCode, CourseName, CreditHours, Description, IsActive FROM Course";
        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var courses = new List<Course>();
        while (await reader.ReadAsync())
        {
            courses.Add(new Course
            {
                Id = reader.GetGuid(0),
                CourseCode = reader.GetString(1),
                CourseName = reader.GetString(2),
                CreditHours = (double)reader.GetDecimal(3),
                Description = reader.GetString(4),
                IsActive = reader.GetBoolean(5)
            });
        }
        return courses;
    }

    public void Add(Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "INSERT INTO Course (Id, CourseCode, CourseName, CreditHours, Description, IsActive) " +
                     "VALUES (@Id, @CourseCode, @CourseName, @CreditHours, @Description, @IsActive)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 3).Value = entity.CourseCode;
        insertCommand.Parameters.Add("@CourseName", SqlDbType.NChar, 10).Value = entity.CourseName;
        insertCommand.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = entity.CreditHours;
        insertCommand.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        insertCommand.ExecuteNonQuery();
    }

    public async Task AddAsync(Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO Course (Id, CourseCode, CourseName, CreditHours, Description, IsActive) " +
                     "VALUES (@Id, @CourseCode, @CourseName, @CreditHours, @Description, @IsActive)";

        using var insertCommand = new SqlCommand(sql, connection);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 3).Value = entity.CourseCode;
        insertCommand.Parameters.Add("@CourseName", SqlDbType.NChar, 10).Value = entity.CourseName;
        insertCommand.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = entity.CreditHours;
        insertCommand.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        await insertCommand.ExecuteNonQueryAsync();
    }

    public void Update(Guid id, Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "UPDATE Course SET CourseCode = @CourseCode, CourseName = @CourseName, " +
                     "CreditHours = @CreditHours, Description = @Description, IsActive = @IsActive " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 3).Value = entity.CourseCode;
        updateCommand.Parameters.Add("@CourseName", SqlDbType.NChar, 10).Value = entity.CourseName;
        updateCommand.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = entity.CreditHours;
        updateCommand.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description;
        updateCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        updateCommand.ExecuteNonQuery();
    }

    public async Task UpdateAsync(Guid id, Course entity)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "UPDATE Course SET CourseCode = @CourseCode, CourseName = @CourseName, " +
                     "CreditHours = @CreditHours, Description = @Description, IsActive = @IsActive " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, connection);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@CourseCode", SqlDbType.NVarChar, 3).Value = entity.CourseCode;
        updateCommand.Parameters.Add("@CourseName", SqlDbType.NChar, 10).Value = entity.CourseName;
        updateCommand.Parameters.Add("@CreditHours", SqlDbType.Decimal).Value = entity.CreditHours;
        updateCommand.Parameters.Add("@Description", SqlDbType.NVarChar).Value = entity.Description;
        updateCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        await updateCommand.ExecuteNonQueryAsync();
    }

    public void Delete(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        connection.Open();

        string sql = "DELETE FROM Course WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        deleteCommand.ExecuteNonQuery();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM Course WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, connection);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        await deleteCommand.ExecuteNonQueryAsync();
    }
}
