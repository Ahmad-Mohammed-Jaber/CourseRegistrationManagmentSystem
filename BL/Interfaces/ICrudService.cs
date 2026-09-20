namespace BL.Interfaces;

public interface ICrudService<TEntity>
{
    TEntity? GetById(int id);

   Task<TEntity?> GetByIdAsync(int id);

    List<TEntity> GetAll();

    Task<List<TEntity>> GetAllAsync();

    void Add(TEntity entity);

    Task AddAsync(TEntity entity);

    void Update(int id, TEntity entity);

    Task UpdateAsync(int id, TEntity entity);

    void Delete(int id);

    Task DeleteAsync(int id);

    List<TEntity> Search(string regex);

    Task<List<TEntity>> SearchAsync(string regex);
}
