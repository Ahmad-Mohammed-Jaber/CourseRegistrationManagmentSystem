using Shared.Entities;

namespace DAL.Interfaces;

public interface IGenericRepository<T> where T : class
{
    T? GetById(int id);

    Task<T?> GetByIdAsync(int id);

    List<T> GetAll();

    Task<List<T>> GetAllAsync();

    int Add(T entity);

    Task<int> AddAsync(T entity);

    int Update(int id, T entity);

    Task<int> UpdateAsync(int id, T entity);

    int Delete(int id);

    Task<int> DeleteAsync(int id);

    List<T> Search(string regex);

    Task<List<T>> SearchAsync(string regex);
}
