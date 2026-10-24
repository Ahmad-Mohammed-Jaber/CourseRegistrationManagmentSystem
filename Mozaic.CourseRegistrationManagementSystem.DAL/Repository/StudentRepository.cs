using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class StudentRepository : IGenericRepository<Student>
{
    public Task<StudentResult> GetAsync(
        int? id = null,
        int? userId = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        return StudentDataProvider.GetAsync(id, userId, search, pageNumber, pageSize);
    }
    public Task<Student?> GetByIdAsync(int id)
    {
        return StudentDataProvider.GetByIdAsync(id);
    }

    public void Add(Student entity)
    {
        StudentDataProvider.Add(entity);
    }

    public Task AddAsync(Student entity)
    {
        return StudentDataProvider.AddAsync(entity);
    }

    public void Update(Student entity)
    {
        StudentDataProvider.Update(entity);
    }

    public Task UpdateAsync(Student entity)
    {
        return StudentDataProvider.UpdateAsync(entity);
    }

    public void Delete(int id)
    {
        StudentDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
    {
        return StudentDataProvider.DeleteAsync(id);
    }

}
