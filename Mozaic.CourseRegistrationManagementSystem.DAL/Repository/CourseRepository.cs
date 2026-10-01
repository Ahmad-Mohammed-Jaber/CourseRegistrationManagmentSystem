using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class CourseRepository : IGenericRepository<Course>
{
    public Course? GetById(int id)
    {
        return CourseDataProvider.GetById(id);
    }

    public Task<Course?> GetByIdAsync(int id)
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

    public List<Course> Search(string regex)
    {
        return CourseDataProvider.Search(regex);
    }

    public Task<List<Course>> SearchAsync(string regex)
    {
        return CourseDataProvider.SearchAsync(regex);
    }
}
