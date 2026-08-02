using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Repository;
using System.Linq;
using System.Text.RegularExpressions;

public class UserService : IGenericService<UserDto>
{
    private readonly UserRepository _userRepository = new UserRepository();

    public UserDto? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        var user = _userRepository.GetById(id);
        return user.ToDto();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        var user = await _userRepository.GetByIdAsync(id);
        return user.ToDto();
    }

    public List<UserDto> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _userRepository.GetAll().Select(user => user.ToDto()!).ToList();
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var users = await _userRepository.GetAllAsync();
        return users.Select(user => user.ToDto()!).ToList();
    }

    public void Add(UserDto userDto)
    {
        AccessValidator.RequireAdmin();
        var user = userDto.ToEntity();
        _userRepository.Add(user);
    }

    public async Task AddAsync(UserDto userDto)
    {
        AccessValidator.RequireAdmin();
        var user = userDto.ToEntity();
        await _userRepository.AddAsync(user);
    }

    public void Update(Guid id, UserDto userDto)
    {
        AccessValidator.RequireAdmin();
        var user = _userRepository.GetById(id);
        if (user == null) throw new KeyNotFoundException($"User with id {id} not found.");

        user.UserName = userDto.UserName;
        user.FullName = userDto.FullName;
        user.Role = userDto.Role;
        user.IsActive = userDto.IsActive;

        _userRepository.Update(id, user);
    }

    public async Task UpdateAsync(Guid id, UserDto userDto)
    {
        AccessValidator.RequireAdmin();
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) throw new KeyNotFoundException($"User with id {id} not found.");

        user.UserName = userDto.UserName;
        user.FullName = userDto.FullName;
        user.Role = userDto.Role;
        user.IsActive = userDto.IsActive;

        await _userRepository.UpdateAsync(id, user);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _userRepository.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _userRepository.DeleteAsync(id);
    }

    public List<UserDto> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var users = _userRepository.GetAll();
        return users
            .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
            .Select(user => user.ToDto()!)
            .ToList();
    }

    public async Task<List<UserDto>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var users = await _userRepository.GetAllAsync();
        return users
            .Where(user => Regex.IsMatch(user.UserName, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(user.FullName, regex, RegexOptions.IgnoreCase))
            .Select(user => user.ToDto()!)
            .ToList();
    }
}
