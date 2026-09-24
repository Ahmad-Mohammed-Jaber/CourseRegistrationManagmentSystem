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

    public RequestResult Add(Course entity)
    {
        return CourseDataProvider.Add(entity);
    }

    public Task<RequestResult> AddAsync(Course entity)
    {
        return CourseDataProvider.AddAsync(entity);
    }

    public RequestResult Update(int id, Course entity)
    {
        return CourseDataProvider.Update(id, entity);
    }

    public Task<RequestResult> UpdateAsync(int id, Course entity)
    {
        return CourseDataProvider.UpdateAsync(id, entity);
    }

    public RequestResult Delete(int id)
    {
        return CourseDataProvider.Delete(id);
    }

    public Task<RequestResult> DeleteAsync(int id)
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