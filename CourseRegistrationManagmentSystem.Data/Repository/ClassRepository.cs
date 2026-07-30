using System.Data;
using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Models;
using Microsoft.Data.SqlClient;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class ClassRepository : IGenericRepository<Class>
{
    public void Add(Class entity)
    {
        using var con = DBConnectionFactory.CreateConnection();
        con.Open();

        string sql = "INSERT INTO Class " +
            "(Id, CourseId, ClassName, Instructor, Capacity, StartDate, EndDate, Schedule, IsActive) " +
            "VALUES (@Id, @CourseId, @ClassName, @Instructor, @Capacity, @StartDate, @EndDate, @Schedule, @IsActive);";

        using var insertCommand = new SqlCommand(sql, con);

        insertCommand.CommandType = CommandType.Text;

        SqlParameter param = new SqlParameter()
        {
            ParameterName = "@Id",
            SqlDbType = SqlDbType.UniqueIdentifier,
            Value = entity.Id,
            Direction = ParameterDirection.Input,
        };

        insertCommand.Parameters.Add(param);

        param = new SqlParameter()
        {
            ParameterName = "@CourseId",
            SqlDbType = SqlDbType.UniqueIdentifier,
            Value = entity.CourseId,
            Direction = ParameterDirection.Input,
        };

        insertCommand.Parameters.Add(param);

        param = new SqlParameter()
        {
            ParameterName = "@ClassName",
            SqlDbType = SqlDbType.NVarChar,
            Size = 50,
            Value = entity.ClassName,
            Direction = ParameterDirection.Input,
        };

        insertCommand.Parameters.Add(param);

        param = new SqlParameter()
        {
            ParameterName = "@Capacity",
            SqlDbType = SqlDbType.Int,
            Value = entity.Capacity,
            Direction = ParameterDirection.Input
        };

        insertCommand.Parameters.Add(param);


        param = new SqlParameter()
        {
            ParameterName = "@StartDate",
            SqlDbType = SqlDbType.DateTime2,
            Value = entity.StartDate,
            Direction = ParameterDirection.Input,
        };

        insertCommand.Parameters.Add(param);

        param = new SqlParameter()
        {
            ParameterName = "@EndDate",
            SqlDbType = SqlDbType.DateTime2,
            Value = entity.EndDate,
            Direction = ParameterDirection.Input,
        };

        insertCommand.Parameters.Add(param);

        param = new SqlParameter()
        {
            ParameterName = "@IsActive",
            SqlDbType = SqlDbType.Bit,
            Value = entity.IsActive,
            Direction = ParameterDirection.Input,
        };

        insertCommand.Parameters.Add(param);

        insertCommand.ExecuteNonQuery();
    }

    public Task AddAsync(Class entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<Class> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<Class>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Class? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Class?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public void Update(Guid id, Class entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, Class entity)
    {
        throw new NotImplementedException();
    }
}
