using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class UserManager
{
    private readonly UserRepository _userRepository = new UserRepository();

    public User? GetById(int id) => _userRepository.GetById(id);

    public Task<User?> GetByIdAsync(int id) => _userRepository.GetByIdAsync(id);

    public Task<User?> GetByUserNameAsync(string userName) => _userRepository.GetByUserNameAsync(userName);

    public List<User> GetAll() => _userRepository.GetAll();

    public Task<List<User>> GetAllAsync() => _userRepository.GetAllAsync();

    public int Add(User user) => _userRepository.Add(user);

    public Task<int > AddAsync(User user) => _userRepository.AddAsync(user);

    public int Update(int id, User user) => _userRepository.Update(id, user);

    public Task<int > UpdateAsync(int id, User user) => _userRepository.UpdateAsync(id, user);

    public int Delete(int id) => _userRepository.Delete(id);

    public Task<int > DeleteAsync(int id) => _userRepository.DeleteAsync(id);

    public List<User> Search(string regex) => _userRepository.Search(regex);

    public Task<List<User>> SearchAsync(string regex) => _userRepository.SearchAsync(regex);
}
