namespace CourseRegistrationManagmentSystem.Business.Interfaces;

public interface ICrudService<TEntity>
{
    TEntity? GetById(Guid id);

    Task<TEntity?> GetByIdAsync(Guid id);

    List<TEntity> GetAll();

    Task<List<TEntity>> GetAllAsync();

    void Add(TEntity entity);

    Task AddAsync(TEntity entity);

    void Update(Guid id, TEntity entity);

    Task UpdateAsync(Guid id, TEntity entity);

    void Delete(Guid id);

    Task DeleteAsync(Guid id);

    List<TEntity> Search(string regex);

    Task<List<TEntity>> SearchAsync(string regex);
}
