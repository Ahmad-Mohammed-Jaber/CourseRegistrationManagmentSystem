using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class ClassRepository : IGenericRepository<Class>
{
    public Class? GetById(int id)
    {
        return ClassDataProvider.GetById(id);
    }

    public Task<Class?> GetByIdAsync(int id)
    {
        return ClassDataProvider.GetByIdAsync(id);
    }

    public List<Class> GetClassesByCourseId(int courseId)
    {
        return ClassDataProvider.GetClassesByCourseId(courseId);
    }

    public Task<List<Class>> GetClassesByCourseIdAsync(int courseId)
    {
        return ClassDataProvider.GetClassesByCourseIdAsync(courseId);
    }

    public List<Class> GetAll()
    {
        return ClassDataProvider.GetAll();
    }

    public Task<List<Class>> GetAllAsync()
    {
        return ClassDataProvider.GetAllAsync();
    }

    /// <summary>
    /// Paged GetAll overload: the page envelope comes straight from
    /// dbo.usp_GetAllClasses (rows plus counters second result set).
    /// </summary>
    public Task<ClassBrowseResult> GetAllAsync(int? studentId, int pageNumber, int pageSize)
    {
        return ClassDataProvider.GetAllAsync(studentId, pageNumber, pageSize);
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

    public List<Class> Search(string regex)
    {
        return ClassDataProvider.Search(regex);
    }

    public Task<List<Class>> SearchAsync(string regex)
    {
        return ClassDataProvider.SearchAsync(regex);
    }
}
