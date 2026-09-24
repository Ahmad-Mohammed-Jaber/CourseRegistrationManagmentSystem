using DAL.Interfaces;
using DAL.Providers;
using Shared.DTOs;
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

    public StudentProfile? GetProfileByUserId(int userId)
    {
        return StudentDataProvider.GetProfileByUserId(userId);
    }

    public Task<StudentProfile?> GetProfileByUserIdAsync(int userId)
    {
        return StudentDataProvider.GetProfileByUserIdAsync(userId);
    }

    public List<Student> GetAll()
    {
        return StudentDataProvider.GetAll();
    }

    public Task<List<Student>> GetAllAsync()
    {
        return StudentDataProvider.GetAllAsync();
    }

    public RequestResult Add(Student entity)
    {
        return StudentDataProvider.Add(entity);
    }

    public Task<RequestResult> AddAsync(Student entity)
    {
        return StudentDataProvider.AddAsync(entity);
    }

    public RequestResult Update(int id, Student entity)
    {
        return StudentDataProvider.Update(id, entity);
    }

    public Task<RequestResult> UpdateAsync(int id, Student entity)
    {
        return StudentDataProvider.UpdateAsync(id, entity);
    }

    public RequestResult Delete(int id)
    {
        return StudentDataProvider.Delete(id);
    }

    public Task<RequestResult> DeleteAsync(int id)
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