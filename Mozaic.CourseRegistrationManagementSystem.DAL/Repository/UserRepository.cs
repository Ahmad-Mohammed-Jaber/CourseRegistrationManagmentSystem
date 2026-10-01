using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class UserRepository : IGenericRepository<User>
{
    public User? GetById(int id)
    {
        return UserDataProvider.GetById(id);
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return UserDataProvider.GetByIdAsync(id);
    }

    public Task<User?> GetByUserNameAsync(string userName)
    {
        return UserDataProvider.GetByUserNameAsync(userName);
    }

    public List<User> GetAll()
    {
        return UserDataProvider.GetAll();
    }

    public Task<List<User>> GetAllAsync()
    {
        return UserDataProvider.GetAllAsync();
    }

    public int Add(User entity)
    {
        return UserDataProvider.Add(entity);
    }

    public Task<int> AddAsync(User entity)
    {
        return UserDataProvider.AddAsync(entity);
    }

    public int Update(int id, User entity)
    {
        return UserDataProvider.Update(id, entity);
    }

    public Task<int> UpdateAsync(int id, User entity)
    {
        return UserDataProvider.UpdateAsync(id, entity);
    }

    public int Delete(int id)
    {
        return UserDataProvider.Delete(id);
    }

    public Task<int> DeleteAsync(int id)
    {
        return UserDataProvider.DeleteAsync(id);
    }

    public List<User> Search(string regex)
    {
        return UserDataProvider.Search(regex);
    }

    public Task<List<User>> SearchAsync(string regex)
    {
        return UserDataProvider.SearchAsync(regex);
    }
}
