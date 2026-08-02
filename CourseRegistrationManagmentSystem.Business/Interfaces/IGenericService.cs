namespace CourseRegistrationManagmentSystem.Business.Interfaces;

public interface IGenericService<T>
{
    T? GetById(Guid id);

    T? GetByIdAsync(Guid id);

    List<T> GetAll();

    List<T> GetAllAsync();

    Task Add(T entity);

    Task AddAsync(T entity);

    Task Update(Guid id, T entity);

    Task UpdateAsync(Guid id, T entity);

    Task Delete(Guid id);

    Task DeleteAsync(Guid id);
}
