using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class UserRepository : IGenericRepository<User>
{
    public Task<UserResult> GetAsync(
        int? id = null,
        string? userName = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        return UserDataProvider.GetAsync(id, userName, search, pageNumber, pageSize);
    }
    public Task<User?> GetByIdAsync(int id)
    {
        return UserDataProvider.GetByIdAsync(id);
    }

    public void Add(User entity)
    {
        UserDataProvider.Add(entity);
    }

    public Task AddAsync(User entity)
    {
        return UserDataProvider.AddAsync(entity);
    }

    public void Update(User entity)
    {
        UserDataProvider.Update(entity);
    }

    public Task UpdateAsync(User entity)
    {
        return UserDataProvider.UpdateAsync(entity);
    }

    public void Delete(int id)
    {
        UserDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
    {
        return UserDataProvider.DeleteAsync(id);
    }

}
