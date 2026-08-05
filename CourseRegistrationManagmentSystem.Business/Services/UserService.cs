using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Managers;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Models;
using System.Text.RegularExpressions;

namespace CourseRegistrationManagmentSystem.Business.Services;

public class UserService : ICrudService<User>
{
    private readonly UserManager _userManager = new UserManager();

    public User? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        return _userManager.GetById(id);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        return await _userManager.GetByIdAsync(id);
    }

    public List<User> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _userManager.GetAll();
    }

    public async Task<List<User>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        return await _userManager.GetAllAsync();
    }

    public void Add(User user)
    {
        AccessValidator.RequireAdmin();
        // Validation
        if (string.IsNullOrWhiteSpace(user.UserName)) throw new ArgumentException("Username is required.");
        if (string.IsNullOrWhiteSpace(user.FullName)) throw new ArgumentException("Full name is required.");

        _userManager.Add(user);
    }

    public async Task AddAsync(User user)
    {
        AccessValidator.RequireAdmin();
        // Validation
        if (string.IsNullOrWhiteSpace(user.UserName)) throw new ArgumentException("Username is required.");
        if (string.IsNullOrWhiteSpace(user.FullName)) throw new ArgumentException("Full name is required.");

        await _userManager.AddAsync(user);
    }

    public void Update(Guid id, User user)
    {
        AccessValidator.RequireAdmin();
        var existingUser = _userManager.GetById(id);
        if (existingUser == null) throw new KeyNotFoundException($"User with id {id} not found.");

        // Validation
        if (string.IsNullOrWhiteSpace(user.UserName)) throw new ArgumentException("Username is required.");
        if (string.IsNullOrWhiteSpace(user.FullName)) throw new ArgumentException("Full name is required.");

        _userManager.Update(id, user);
    }

    public async Task UpdateAsync(Guid id, User user)
    {
        AccessValidator.RequireAdmin();
        var existingUser = await _userManager.GetByIdAsync(id);
        if (existingUser == null) throw new KeyNotFoundException($"User with id {id} not found.");

        // Validation
        if (string.IsNullOrWhiteSpace(user.UserName)) throw new ArgumentException("Username is required.");
        if (string.IsNullOrWhiteSpace(user.FullName)) throw new ArgumentException("Full name is required.");

        await _userManager.UpdateAsync(id, user);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _userManager.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _userManager.DeleteAsync(id);
    }

    public List<User> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var users = _userManager.GetAll();
        return users
            .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                           Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
            .ToList();
    }

    public async Task<List<User>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var users = await _userManager.GetAllAsync();
        return users
            .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                           Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
            .ToList();
    }
}
