using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class UserManager
{

    private readonly UserRepository _userRepository = new UserRepository();
    public Task<UserResult> GetAsync(
        int? id = null,
        string? userName = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null) => _userRepository.GetAsync(id, userName, search, pageNumber, pageSize);

    public Task<User?> GetByIdAsync(int id) => _userRepository.GetByIdAsync(id);

    public void Add(User user) => _userRepository.Add(user);

    public Task AddAsync(User user) => _userRepository.AddAsync(user);

    public void Update(User user) => _userRepository.Update(user);

    public Task UpdateAsync(User user) => _userRepository.UpdateAsync(user);

    public void Delete(int id) => _userRepository.Delete(id);

    public Task DeleteAsync(int id) => _userRepository.DeleteAsync(id);

}
