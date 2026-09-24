using Shared.Entities;

namespace DAL.Interfaces;

public interface IGenericProvider<T> where T : class
{
    T? GetById(int id);

    Task<T?> GetByIdAsync(int id);

    List<T> GetAll();

    Task<List<T>> GetAllAsync();

    RequestResult Add(T entity);

    Task<RequestResult> AddAsync(T entity);

    RequestResult Update(int id, T entity);

    Task<RequestResult> UpdateAsync(int id, T entity);

    RequestResult Delete(int id);

    Task<RequestResult> DeleteAsync(int id);

    List<T> Search(string regex);

    Task<List<T>> SearchAsync(string regex);
}