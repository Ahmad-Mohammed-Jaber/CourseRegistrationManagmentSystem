using Shared.Entities;

namespace BL.Interfaces;

public interface ICrudService<TEntity>
{
    Result<TEntity?> GetById(int id);

    Task<Result<TEntity?>> GetByIdAsync(int id);

    Result<List<TEntity>> GetAll();

    Task<Result<List<TEntity>>> GetAllAsync();

    ValidationResult Add(TEntity entity);

    Task<ValidationResult> AddAsync(TEntity entity);

    ValidationResult Update(int id, TEntity entity);

    Task<ValidationResult> UpdateAsync(int id, TEntity entity);

    ValidationResult Delete(int id);

    Task<ValidationResult> DeleteAsync(int id);

    Result<List<TEntity>> Search(string regex);

    Task<Result<List<TEntity>>> SearchAsync(string regex);
}
