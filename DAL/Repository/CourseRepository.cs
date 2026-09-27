using DAL.Interfaces;
using DAL.Providers;
using Shared.Entities;

namespace DAL.Repository;

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

    public int Add(Course entity)
    {
        return CourseDataProvider.Add(entity);
    }

    public Task<int> AddAsync(Course entity)
    {
        return CourseDataProvider.AddAsync(entity);
    }

    public int Update(int id, Course entity)
    {
        return CourseDataProvider.Update(id, entity);
    }

    public Task<int> UpdateAsync(int id, Course entity)
    {
        return CourseDataProvider.UpdateAsync(id, entity);
    }

    public int Delete(int id)
    {
        return CourseDataProvider.Delete(id);
    }

    public Task<int> DeleteAsync(int id)
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
