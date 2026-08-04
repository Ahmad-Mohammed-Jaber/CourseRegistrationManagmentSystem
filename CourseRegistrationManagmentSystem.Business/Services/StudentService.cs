using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Data.Repository;
using System.Text.RegularExpressions;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Services;

public class StudentService : ICrudService<StudentDto>
{
    private readonly StudentRepository _studentRepository = new StudentRepository();
    private readonly UserRepository _userRepository = new UserRepository();

    public StudentDto? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        var student = _studentRepository.GetById(id);
        if (student == null) return null;
        var user = _userRepository.GetById(student.UserId);
        return student.ToDto(user);
    }

    public async Task<StudentDto?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null) return null;
        var user = await _userRepository.GetByIdAsync(student.UserId);
        return student.ToDto(user);
    }

    public List<StudentDto> GetAll()
    {
        AccessValidator.RequireAdmin();
        var students = _studentRepository.GetAll();
        var users = _userRepository.GetAll();
        return students.Select(student =>
        {
            var user = users.FirstOrDefault(u => u.Id == student.UserId);
            return student.ToDto(user)!;
        }).ToList();
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var students = await _studentRepository.GetAllAsync();
        var users = await _userRepository.GetAllAsync();
        return students.Select(student =>
        {
            var user = users.FirstOrDefault(u => u.Id == student.UserId);
            return student.ToDto(user)!;
        }).ToList();
    }

    public void Add(StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();
        var student = studentDto.ToEntity();

        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FullName = studentDto.FullName,
            UserName = studentDto.UserName,
            IsActive = studentDto.IsActive,
            Role = User.UserRoles.Student,
            PasswordHash = "Fix this lol",
        };

        student.UserId = userId;

        _userRepository.Add(user);
        _studentRepository.Add(student);
    }

    public async Task AddAsync(StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();
        var student = studentDto.ToEntity();

        Guid userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            FullName = studentDto.FullName,
            UserName = studentDto.UserName,
            IsActive = studentDto.IsActive,
            Role = User.UserRoles.Student,
            PasswordHash = "Fix this lol",
        };

        student.UserId = userId;

        await _userRepository.AddAsync(user);
        await _studentRepository.AddAsync(student);
    }

    public void Update(Guid id, StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();

        var student = _studentRepository.GetById(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        var user = _userRepository.GetById(student.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User with id {student.UserId} not found.");

        student.StudentNumber = studentDto.StudentNumber;
        student.Email = studentDto.Email;
        student.Phone = studentDto.Phone;

        user.FullName = studentDto.FullName;
        user.UserName = studentDto.UserName;
        user.IsActive = studentDto.IsActive;

        _studentRepository.Update(student.Id, student);
        _userRepository.Update(user.Id, user);
    }

    public async Task UpdateAsync(Guid id, StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();

        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        var user = await _userRepository.GetByIdAsync(student.UserId);
        if (user == null)
            throw new KeyNotFoundException($"User with id {student.UserId} not found.");

        student.StudentNumber = studentDto.StudentNumber;
        student.Email = studentDto.Email;
        student.Phone = studentDto.Phone;

        user.FullName = studentDto.FullName;
        user.UserName = studentDto.UserName;
        user.IsActive = studentDto.IsActive;

        await _studentRepository.UpdateAsync(student.Id, student);
        await _userRepository.UpdateAsync(user.Id, user);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();

        var student = _studentRepository.GetById(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        _studentRepository.Delete(id);
        _userRepository.Delete(student.UserId);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();

        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        await _studentRepository.DeleteAsync(id);
        await _userRepository.DeleteAsync(student.UserId);
    }

    public List<StudentDto> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var students = _studentRepository.Search(regex);
        var users = _userRepository.GetAll();
        return students
            .Select(student =>
            {
                var user = users.FirstOrDefault(u => u.Id == student.UserId);
                return student.ToDto(user)!;
            })
            .ToList();
    }

    public async Task<List<StudentDto>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var students = await _studentRepository.SearchAsync(regex);
        var users = await _userRepository.GetAllAsync();
        return students
            .Select(student =>
            {
                var user = users.FirstOrDefault(u => u.Id == student.UserId);
                return student.ToDto(user)!;
            })
            .ToList();
    }
}
