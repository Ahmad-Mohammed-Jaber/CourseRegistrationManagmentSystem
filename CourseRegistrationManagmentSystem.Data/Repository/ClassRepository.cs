using CourseRegistrationManagmentSystem.Shared.Models;
using DAL.Interfaces;
using DAL.Providers;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class ClassRepository : IGenericRepository<Class>
{
    public Class? GetById(Guid id)
    {
        return ClassDataProvider.GetById(id);
    }

    public Task<Class?> GetByIdAsync(Guid id)
    {
        return ClassDataProvider.GetByIdAsync(id);
    }

    public List<Class> GetClassesByCourseId(Guid courseId)
    {
        return ClassDataProvider.GetClassesByCourseId(courseId);
    }

    public Task<List<Class>> GetClassesByCourseIdAsync(Guid courseId)
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

    public void Add(Class entity)
    {
        ClassDataProvider.Add(entity);
    }

    public Task AddAsync(Class entity)
    {
        return ClassDataProvider.AddAsync(entity);
    }

    public void Update(Guid id, Class entity)
    {
        ClassDataProvider.Update(id, entity);
    }

    public Task UpdateAsync(Guid id, Class entity)
    {
        return ClassDataProvider.UpdateAsync(id, entity);
    }

    public void Delete(Guid id)
    {
        ClassDataProvider.Delete(id);
    }

    public Task DeleteAsync(Guid id)
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
