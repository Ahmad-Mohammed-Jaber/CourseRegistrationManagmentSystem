namespace CourseRegistrationManagmentSystem.Business.Interfaces;

public interface IGenericService<TEntity, TDto>
{
    TDto? GetById(Guid id);

    TDto? GetByIdAsync(Guid id);

    List<TDto> GetAll();

    List<TDto> GetAllAsync();

    Task Add(TDto dto);

    Task AddAsync(TDto dto);

    Task Update(Guid id, TDto dto);

    Task UpdateAsync(Guid id, TDto dto);

    Task Delete(Guid id);

    Task DeleteAsync(Guid id);
}
