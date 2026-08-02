using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Models;
public class UserService : IGenericService<User>
{
    public User? GetById(Guid id)
    {

    }

    public User? GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public List<User> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task Add(User entity)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(User entity)
    {
        throw new NotImplementedException();
    }

    public Task Update(Guid id, User entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, User entity)
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
