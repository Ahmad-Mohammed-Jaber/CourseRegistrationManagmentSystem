using DAL.Interfaces;
using DAL.Providers;
using Shared.Entities;

namespace DAL.Repository;

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

    public void Add(User entity)
    {
        UserDataProvider.Add(entity);
    }

    public Task AddAsync(User entity)
    {
        return UserDataProvider.AddAsync(entity);
    }

    public void Update(int id, User entity)
    {
        UserDataProvider.Update(id, entity);
    }

    public Task UpdateAsync(int id, User entity)
    {
        return UserDataProvider.UpdateAsync(id, entity);
    }

    public void Delete(int id)
    {
        UserDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
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