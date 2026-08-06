using CourseRegistrationManagmentSystem.Data.Providers;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Interfaces;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class StudentRepository : IGenericRepository<Student>
{
    public Student? GetById(Guid id)
    {
        return StudentDataProvider.GetById(id);
    }

    public Task<Student?> GetByIdAsync(Guid id)
    {
        return StudentDataProvider.GetByIdAsync(id);
    }

    public Task<Student?> GetByUserIdAsync(Guid userId)
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

    public void Update(Guid id, Student entity)
    {
        StudentDataProvider.Update(id, entity);
    }

    public Task UpdateAsync(Guid id, Student entity)
    {
        return StudentDataProvider.UpdateAsync(id, entity);
    }

    public void Delete(Guid id)
    {
        StudentDataProvider.Delete(id);
    }

    public Task DeleteAsync(Guid id)
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
