using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Managers;

internal class UserManager
{
    private readonly UserRepository _userRepository = new UserRepository();

    public User? GetById(Guid id) => _userRepository.GetById(id);

    public Task<User?> GetByIdAsync(Guid id) => _userRepository.GetByIdAsync(id);

    public Task<User?> GetByUserNameAsync(string userName) => _userRepository.GetByUserNameAsync(userName);

    public List<User> GetAll() => _userRepository.GetAll();

    public Task<List<User>> GetAllAsync() => _userRepository.GetAllAsync();

    public void Add(User user) => _userRepository.Add(user);

    public Task AddAsync(User user) => _userRepository.AddAsync(user);

    public void Update(Guid id, User user) => _userRepository.Update(id, user);

    public Task UpdateAsync(Guid id, User user) => _userRepository.UpdateAsync(id, user);

    public void Delete(Guid id) => _userRepository.Delete(id);

    public Task DeleteAsync(Guid id) => _userRepository.DeleteAsync(id);

    public List<User> Search(string regex) => _userRepository.Search(regex);

    public Task<List<User>> SearchAsync(string regex) => _userRepository.SearchAsync(regex);
}
