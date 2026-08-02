using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Models;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Dtos;

public class UserService : IGenericService<User, UserDto>
{
    public UserDto? GetById(Guid id)
    {
        // Implementation would call repository and use .ToDto()
        throw new NotImplementedException();
    }

    public UserDto? GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<UserDto> GetAll()
    {
        throw new NotImplementedException();
    }

    public List<UserDto> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task Add(UserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(UserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task Update(Guid id, UserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, UserDto dto)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
