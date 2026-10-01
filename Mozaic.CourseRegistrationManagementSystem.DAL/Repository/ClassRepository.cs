using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

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

    public int Add(Class entity)
    {
        return ClassDataProvider.Add(entity);
    }

    public Task<int> AddAsync(Class entity)
    {
        return ClassDataProvider.AddAsync(entity);
    }

    public int Update(int id, Class entity)
    {
        return ClassDataProvider.Update(id, entity);
    }

    public Task<int> UpdateAsync(int id, Class entity)
    {
        return ClassDataProvider.UpdateAsync(id, entity);
    }

    public int Delete(int id)
    {
        return ClassDataProvider.Delete(id);
    }

    public Task<int> DeleteAsync(int id)
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
