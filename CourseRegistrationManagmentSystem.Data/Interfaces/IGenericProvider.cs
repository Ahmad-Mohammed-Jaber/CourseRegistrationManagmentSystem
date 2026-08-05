namespace CourseRegistrationManagmentSystem.Data.Interfaces;

public interface IGenericProvider<T> where T : class
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

    List<T> Search(string regex);

    Task<List<T>> SearchAsync(string regex);
}
