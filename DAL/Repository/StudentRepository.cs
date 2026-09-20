using DAL.Interfaces;
using DAL.Providers;
using Shared.Entities;

namespace DAL.Repository;

public class StudentRepository : IGenericRepository<Student>
{
    public Student? GetById(int id)
    {
        return StudentDataProvider.GetById(id);
    }

    public Task<Student?> GetByIdAsync(int id)
    {
        return StudentDataProvider.GetByIdAsync(id);
    }

    public Student? GetByUserId(int userId)
    {
        return StudentDataProvider.GetByUserId(userId);
    }

    public Task<Student?> GetByUserIdAsync(int userId)
    {
        return StudentDataProvider.GetByUserIdAsync(userId);
    }

    public List<Student> GetAll()
    {
        return StudentDataProvider.GetAll();
    }

    public Task<List<Student>> GetAllAsync()
    {
        return StudentDataProvider.GetAllAsync();
    }

    public void Add(Student entity)
    {
        StudentDataProvider.Add(entity);
    }

    public Task AddAsync(Student entity)
    {
        return StudentDataProvider.AddAsync(entity);
    }

    public void Update(int id, Student entity)
    {
        StudentDataProvider.Update(id, entity);
    }

    public Task UpdateAsync(int id, Student entity)
    {
        return StudentDataProvider.UpdateAsync(id, entity);
    }

    public void Delete(int id)
    {
        StudentDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
    {
        return StudentDataProvider.DeleteAsync(id);
    }

    public List<Student> Search(string regex)
    {
        return StudentDataProvider.Search(regex);
    }

    public Task<List<Student>> SearchAsync(string regex)
    {
        return StudentDataProvider.SearchAsync(regex);
    }
}