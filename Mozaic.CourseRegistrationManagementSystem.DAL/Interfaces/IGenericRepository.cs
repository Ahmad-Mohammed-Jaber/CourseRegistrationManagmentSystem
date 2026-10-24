using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;

/// <summary>
/// The write contract shared by every repository. The read contract is not
/// part of this interface: each entity has its own filters, so every
/// repository declares its own single GetAsync with optional filters and
/// optional paging.
/// </summary>
public interface IGenericRepository<T> where T : class
{
    void Add(T entity);

    Task AddAsync(T entity);

    void Update(T entity);

    Task UpdateAsync(T entity);

    void Delete(int id);

    Task DeleteAsync(int id);
}
