using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class ClassRepository : IGenericRepository<Class>
{
    public Task<ClassResult> GetAsync(
        int? id = null,
        int? courseId = null,
        int? userId = null,
        string? search = null,
        string? statusFilter = null,
        string? classNameFilter = null,
        string? instructorFilter = null,
        string? courseFilter = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        return ClassDataProvider.GetAsync(
            id, courseId, userId, search, statusFilter, classNameFilter, instructorFilter,
            courseFilter, pageNumber, pageSize);
    }
    public Task<Class?> GetByIdAsync(int id)
    {
        return ClassDataProvider.GetByIdAsync(id);
    }

    public void Add(Class entity)
    {
        ClassDataProvider.Add(entity);
    }

    public Task AddAsync(Class entity)
    {
        return ClassDataProvider.AddAsync(entity);
    }

    public void Update(Class entity)
    {
        ClassDataProvider.Update(entity);
    }

    public Task UpdateAsync(Class entity)
    {
        return ClassDataProvider.UpdateAsync(entity);
    }

    public void Delete(int id)
    {
        ClassDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
    {
        return ClassDataProvider.DeleteAsync(id);
    }

}
