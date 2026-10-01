using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Dtos;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class StudentRepository : IGenericRepository<Student>
{
    public Student? GetById(int id)
    {
        return StudentDataProvider.GetById(id);
    }

    public Task<Student?> GetByIdAsync(int id)
    {
        return StudentDataProvider.GetByIdAsync(id);
    }

    public Student? GetByUserId(int userId)
    {
        return StudentDataProvider.GetByUserId(userId);
    }

    public Task<Student?> GetByUserIdAsync(int userId)
    {
        return StudentDataProvider.GetByUserIdAsync(userId);
    }

    public StudentProfile? GetProfileByUserId(int userId)
    {
        return StudentDataProvider.GetProfileByUserId(userId);
    }

    public Task<StudentProfile?> GetProfileByUserIdAsync(int userId)
    {
        return StudentDataProvider.GetProfileByUserIdAsync(userId);
    }

    public List<Student> GetAll()
    {
        return StudentDataProvider.GetAll();
    }

    public Task<List<Student>> GetAllAsync()
    {
        return StudentDataProvider.GetAllAsync();
    }

    public int Add(Student entity)
    {
        return StudentDataProvider.Add(entity);
    }

    public Task<int > AddAsync(Student entity)
    {
        return StudentDataProvider.AddAsync(entity);
    }

    public int Update(int id, Student entity)
    {
        return StudentDataProvider.Update(id, entity);
    }

    public Task<int > UpdateAsync(int id, Student entity)
    {
        return StudentDataProvider.UpdateAsync(id, entity);
    }

    public int Delete(int id)
    {
        return StudentDataProvider.Delete(id);
    }

    public Task<int > DeleteAsync(int id)
    {
        return StudentDataProvider.DeleteAsync(id);
    }

    public List<Student> Search(string regex)
    {
        return StudentDataProvider.Search(regex);
    }

    public Task<List<Student>> SearchAsync(string regex)
    {
        return StudentDataProvider.SearchAsync(regex);
    }
}
