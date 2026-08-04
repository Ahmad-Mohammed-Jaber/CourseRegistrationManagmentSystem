using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class ClassRepository : IGenericRepository<Class>
{
    public Class? GetById(Guid id)
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive FROM Class WHERE Id = @Id";
        using var command = new SqlCommand(sql, con);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new Class
            {
                Id = reader.GetGuid(0),
                CourseId = reader.GetGuid(1),
                ClassName = reader.GetString(2),
                Instructor = reader.GetString(3),
                MaxCapacity = reader.GetInt32(4),
                CurrentCapacity = reader.GetInt32(5),
                StartDate = reader.GetDateTime(6),
                EndDate = reader.GetDateTime(7),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(8),
                IsActive = reader.GetBoolean(9)
            };
        }
        return null;
    }

    public async Task<Class?> GetByIdAsync(Guid id)
    {
        using var con = DBConnectionFactory.CreateConnection();
        await con.OpenAsync();

        string sql = "SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive FROM Class WHERE Id = @Id";
        using var command = new SqlCommand(sql, con);
        command.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Class
            {
                Id = reader.GetGuid(0),
                CourseId = reader.GetGuid(1),
                ClassName = reader.GetString(2),
                Instructor = reader.GetString(3),
                MaxCapacity = reader.GetInt32(4),
                CurrentCapacity = reader.GetInt32(5),
                StartDate = reader.GetDateTime(6),
                EndDate = reader.GetDateTime(7),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(8),
                IsActive = reader.GetBoolean(9)
            };
        }
        return null;
    }

    public List<Class> GetAll()
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "SELECT Id, CourseId, ClassName, Instructor, CurrentCapacity, MaxCapacity , StartDate, EndDate, Schedule, IsActive FROM Class";
        using var command = new SqlCommand(sql, con);
        using var reader = command.ExecuteReader();

        var classes = new List<Class>();
        while (reader.Read())
        {
            classes.Add(new Class
            {
                Id = reader.GetGuid(0),
                CourseId = reader.GetGuid(1),
                ClassName = reader.GetString(2),
                Instructor = reader.GetString(3),
                MaxCapacity = reader.GetInt32(4),
                CurrentCapacity = reader.GetInt32(5),
                StartDate = reader.GetDateTime(6),
                EndDate = reader.GetDateTime(7),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(8),
                IsActive = reader.GetBoolean(9)
            });
        }
        return classes;
    }

    public async Task<List<Class>> GetAllAsync()
    {
        using var con = DBConnectionFactory.CreateConnection();
        await con.OpenAsync();

        string sql = @"
        SELECT 
            Id,
            CourseId,
            ClassName,
            Instructor,
            CurrentCapacity,
            MaxCapacity,
            StartDate,
            EndDate,
            Schedule,
            IsActive
        FROM Class";

        using var command = new SqlCommand(sql, con);
        using var reader = await command.ExecuteReaderAsync();

        var classes = new List<Class>();

        while (await reader.ReadAsync())
        {
            classes.Add(new Class
            {
                Id = reader.GetGuid(0),
                CourseId = reader.GetGuid(1),
                ClassName = reader.GetString(2),
                Instructor = reader.GetString(3),

                // Fixed mapping
                CurrentCapacity = reader.GetInt32(4),
                MaxCapacity = reader.GetInt32(5),

                StartDate = reader.GetDateTime(6),
                EndDate = reader.GetDateTime(7),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(8),
                IsActive = reader.GetBoolean(9)
            });
        }

        return classes;
    }
    public void Add(Class entity)
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "INSERT INTO Class " +
            "(Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive) " +
            "VALUES (@Id, @CourseId, @ClassName, @Instructor, @MaxCapacity, @CurrentCapacity, @StartDate, @EndDate, @Schedule, @IsActive);";

        using var insertCommand = new SqlCommand(sql, con);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = entity.CourseId;
        insertCommand.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName;
        insertCommand.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor;
        insertCommand.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
        insertCommand.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
        insertCommand.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
        insertCommand.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
        insertCommand.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        insertCommand.ExecuteNonQuery();
    }

    public async Task AddAsync(Class entity)
    {
        using var con = DBConnectionFactory.CreateConnection();
        await con.OpenAsync();

        string sql = "INSERT INTO Class " +
            "(Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive) " +
            "VALUES (@Id, @CourseId, @ClassName, @Instructor, @MaxCapacity, @CurrentCapacity, @StartDate, @EndDate, @Schedule, @IsActive);";

        using var insertCommand = new SqlCommand(sql, con);
        insertCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = entity.Id;
        insertCommand.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = entity.CourseId;
        insertCommand.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName;
        insertCommand.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor;
        insertCommand.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
        insertCommand.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
        insertCommand.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
        insertCommand.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
        insertCommand.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
        insertCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        await insertCommand.ExecuteNonQueryAsync();
    }

    public void Update(Guid id, Class entity)
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "UPDATE Class SET CourseId = @CourseId, ClassName = @ClassName, Instructor = @Instructor, " +
                     "MaxCapacity = @MaxCapacity, CurrentCapacity = @CurrentCapacity, StartDate = @StartDate, EndDate = @EndDate, Schedule = @Schedule, IsActive = @IsActive " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, con);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = entity.CourseId;
        updateCommand.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName;
        updateCommand.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor;
        updateCommand.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
        updateCommand.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
        updateCommand.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
        updateCommand.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
        updateCommand.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
        updateCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        updateCommand.ExecuteNonQuery();
    }

    public async Task UpdateAsync(Guid id, Class entity)
    {
        using var con = DBConnectionFactory.CreateConnection();
        await con.OpenAsync();

        string sql = "UPDATE Class SET CourseId = @CourseId, ClassName = @ClassName, Instructor = @Instructor, " +
                     "MaxCapacity = @MaxCapacity, CurrentCapacity = @CurrentCapacity, StartDate = @StartDate, EndDate = @EndDate, Schedule = @Schedule, IsActive = @IsActive " +
                     "WHERE Id = @Id";

        using var updateCommand = new SqlCommand(sql, con);
        updateCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;
        updateCommand.Parameters.Add("@CourseId", SqlDbType.UniqueIdentifier).Value = entity.CourseId;
        updateCommand.Parameters.Add("@ClassName", SqlDbType.NVarChar, 50).Value = entity.ClassName;
        updateCommand.Parameters.Add("@Instructor", SqlDbType.NVarChar, 50).Value = entity.Instructor;
        updateCommand.Parameters.Add("@MaxCapacity", SqlDbType.Int).Value = entity.MaxCapacity;
        updateCommand.Parameters.Add("@CurrentCapacity", SqlDbType.Int).Value = entity.CurrentCapacity;
        updateCommand.Parameters.Add("@StartDate", SqlDbType.DateTime2).Value = entity.StartDate;
        updateCommand.Parameters.Add("@EndDate", SqlDbType.DateTime2).Value = entity.EndDate;
        updateCommand.Parameters.Add("@Schedule", SqlDbType.Int).Value = (int)entity.Schedule;
        updateCommand.Parameters.Add("@IsActive", SqlDbType.Bit).Value = entity.IsActive;

        await updateCommand.ExecuteNonQueryAsync();
    }

    public void Delete(Guid id)
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "DELETE FROM Class WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, con);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        deleteCommand.ExecuteNonQuery();
    }

    public async Task DeleteAsync(Guid id)
    {
        using var con = DBConnectionFactory.CreateConnection();
        await con.OpenAsync();

        string sql = "DELETE FROM Class WHERE Id = @Id";
        using var deleteCommand = new SqlCommand(sql, con);
        deleteCommand.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = id;

        await deleteCommand.ExecuteNonQueryAsync();
    }

    public List<Class> Search(string regex)
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive FROM Class WHERE ClassName LIKE @regex OR Instructor LIKE @regex";
        using var command = new SqlCommand(sql, con);
        command.Parameters.Add("@regex", SqlDbType.NVarChar).Value = $"%{regex}%";

        using var reader = command.ExecuteReader();
        var classes = new List<Class>();
        while (reader.Read())
        {
            classes.Add(new Class
            {
                Id = reader.GetGuid(0),
                CourseId = reader.GetGuid(1),
                ClassName = reader.GetString(2),
                Instructor = reader.GetString(3),
                MaxCapacity = reader.GetInt32(4),
                CurrentCapacity = reader.GetInt32(5),
                StartDate = reader.GetDateTime(6),
                EndDate = reader.GetDateTime(7),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(8),
                IsActive = reader.GetBoolean(9)
            });
        }
        return classes;
    }

    public async Task<List<Class>> SearchAsync(string regex)
    {
        using var con = DBConnectionFactory.CreateConnection();
        await con.OpenAsync();

        string sql = "SELECT Id, CourseId, ClassName, Instructor, MaxCapacity, CurrentCapacity, StartDate, EndDate, Schedule, IsActive FROM Class WHERE ClassName LIKE @regex OR Instructor LIKE @regex";
        using var command = new SqlCommand(sql, con);
        command.Parameters.Add("@regex", SqlDbType.NVarChar).Value = $"%{regex}%";

        using var reader = await command.ExecuteReaderAsync();
        var classes = new List<Class>();
        while (await reader.ReadAsync())
        {
            classes.Add(new Class
            {
                Id = reader.GetGuid(0),
                CourseId = reader.GetGuid(1),
                ClassName = reader.GetString(2),
                Instructor = reader.GetString(3),
                MaxCapacity = reader.GetInt32(4),
                CurrentCapacity = reader.GetInt32(5),
                StartDate = reader.GetDateTime(6),
                EndDate = reader.GetDateTime(7),
                Schedule = (Class.DaysOfWeek)reader.GetInt32(8),
                IsActive = reader.GetBoolean(9)
            });
        }
        return classes;
    }
}
