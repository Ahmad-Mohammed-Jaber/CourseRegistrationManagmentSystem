using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);

    Task AddAsync(T entity);

    void Update(T entity);

    Task UpdateAsync(T entity);

    void Delete(int id);

    Task DeleteAsync(int id);
}
