using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;

public interface IGenericRepository<T> where T : class
{
    T? GetById(int id);

    Task<T?> GetByIdAsync(int id);

    List<T> GetAll();

    Task<List<T>> GetAllAsync();

    void Add(T entity);

    Task AddAsync(T entity);

    void Update(T entity);

    Task UpdateAsync(T entity);

    void Delete(int id);

    Task DeleteAsync(int id);

    List<T> Search(string regex);

    Task<List<T>> SearchAsync(string regex);
}
