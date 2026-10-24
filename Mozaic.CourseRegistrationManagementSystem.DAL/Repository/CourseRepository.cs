using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class CourseRepository : IGenericRepository<Course>
{
    public Task<CourseResult> GetAsync(
        int? id = null,
        string? courseCode = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        return CourseDataProvider.GetAsync(id, courseCode, search, pageNumber, pageSize);
    }
    public Task<Course?> GetByIdAsync(int id)
    {
        return CourseDataProvider.GetByIdAsync(id);
    }

    public void Add(Course entity)
    {
        CourseDataProvider.Add(entity);
    }

    public Task AddAsync(Course entity)
    {
        return CourseDataProvider.AddAsync(entity);
    }

    public void Update(Course entity)
    {
        CourseDataProvider.Update(entity);
    }

    public Task UpdateAsync(Course entity)
    {
        return CourseDataProvider.UpdateAsync(entity);
    }

    public void Delete(int id)
    {
        CourseDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
    {
        return CourseDataProvider.DeleteAsync(id);
    }

}
