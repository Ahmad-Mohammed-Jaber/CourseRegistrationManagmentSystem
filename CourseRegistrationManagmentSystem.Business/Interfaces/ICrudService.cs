namespace CourseRegistrationManagmentSystem.Business.Interfaces;

public interface ICrudService<TDto>
{
    TDto? GetById(Guid id);

    Task<TDto?> GetByIdAsync(Guid id);

    List<TDto> GetAll();

    Task<List<TDto>> GetAllAsync();

    void Add(TDto dto);

    Task AddAsync(TDto dto);

    void Update(Guid id, TDto dto);

    Task UpdateAsync(Guid id, TDto dto);

    void Delete(Guid id);

    Task DeleteAsync(Guid id);

    List<TDto> Search(string regex);

    Task<List<TDto>> SearchAsync(string regex);
}
