using DAL.Interfaces;
using DAL.Providers;
using Shared.Entities;

namespace DAL.Repository;

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

    public RequestResult Add(Class entity)
    {
        return ClassDataProvider.Add(entity);
    }

    public Task<RequestResult> AddAsync(Class entity)
    {
        return ClassDataProvider.AddAsync(entity);
    }

    public RequestResult Update(int id, Class entity)
    {
        return ClassDataProvider.Update(id, entity);
    }

    public Task<RequestResult> UpdateAsync(int id, Class entity)
    {
        return ClassDataProvider.UpdateAsync(id, entity);
    }

    public RequestResult Delete(int id)
    {
        return ClassDataProvider.Delete(id);
    }

    public Task<RequestResult> DeleteAsync(int id)
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