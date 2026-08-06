using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Interfaces;
using CourseRegistrationManagmentSystem.Data.Providers;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class CourseRepository : IGenericRepository<Course>
{
    public Course? GetById(Guid id)
    {
        return CourseDataProvider.GetById(id);
    }

    public Task<Course?> GetByIdAsync(Guid id)
    {
        return CourseDataProvider.GetByIdAsync(id);
    }

    public List<Course> GetAll()
    {
        return CourseDataProvider.GetAll();
    }

    public Task<List<Course>> GetAllAsync()
    {
        return CourseDataProvider.GetAllAsync();
    }

    public void Add(Course entity)
    {
        CourseDataProvider.Add(entity);
    }

    public Task AddAsync(Course entity)
    {
        return CourseDataProvider.AddAsync(entity);
    }

    public void Update(Guid id, Course entity)
    {
        CourseDataProvider.Update(id, entity);
    }

    public Task UpdateAsync(Guid id, Course entity)
    {
        return CourseDataProvider.UpdateAsync(id, entity);
    }

    public void Delete(Guid id)
    {
        CourseDataProvider.Delete(id);
    }

    public Task DeleteAsync(Guid id)
    {
        return CourseDataProvider.DeleteAsync(id);
    }

    public List<Course> Search(string regex)
    {
        return CourseDataProvider.Search(regex);
    }

    public Task<List<Course>> SearchAsync(string regex)
    {
        return CourseDataProvider.SearchAsync(regex);
    }
}
