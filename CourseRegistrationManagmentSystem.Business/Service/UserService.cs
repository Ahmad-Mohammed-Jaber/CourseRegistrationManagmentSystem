using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Shared.Dtos;

public class UserService : IGenericService<UserDto>
{
    public UserDto? GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<UserDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<List<UserDto>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public void Add(UserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(UserDto dto)
    {
        throw new NotImplementedException();
    }

    public void Update(Guid id, UserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, UserDto dto)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
