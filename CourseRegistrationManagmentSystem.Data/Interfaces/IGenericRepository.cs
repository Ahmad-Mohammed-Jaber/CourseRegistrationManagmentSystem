namespace CourseRegistrationManagmentSystem.Models;

public interface IGenericRepository<T>
{
    T? GetById(Guid id);
    
    Task<T?> GetByIdAsync(Guid id);

    List<T> GetAll();

    Task<List<T>> GetAllAsync();

    void Add(T entity);

    Task AddAsync(T entity);
    
    void Update(Guid id, T entity);

    Task UpdateAsync(Guid id, T entity);

    void Delete(Guid id);

    Task DeleteAsync(Guid id);
}