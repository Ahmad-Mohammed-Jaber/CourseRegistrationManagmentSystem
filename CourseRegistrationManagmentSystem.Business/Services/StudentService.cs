using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Managers;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Services;

public class StudentService : ICrudService<Student>
{
    private readonly StudentManager _studentManager = new StudentManager();
    private readonly UserManager _userManager = new UserManager();

    public Student? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        return _studentManager.GetById(id);
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        return await _studentManager.GetByIdAsync(id);
    }

    public List<Student> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _studentManager.GetAll();
    }

    public async Task<List<Student>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        return await _studentManager.GetAllAsync();
    }

    public void Add(Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);

        if (student.UserId == Guid.Empty)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
            };
            _userManager.Add(user);
            student.UserId = user.Id;
        }

        _studentManager.Add(student);
    }

    public async Task AddAsync(Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);

        if (student.UserId == Guid.Empty)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
            };
            await _userManager.AddAsync(user);
            student.UserId = user.Id;
        }

        await _studentManager.AddAsync(student);
    }

    public void Update(Guid id, Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);

        var existingStudent = _studentManager.GetById(id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        _studentManager.Update(id, student);

        var existingUser = _userManager.GetById(student.UserId);
        if (existingUser != null)
        {
            existingUser.FullName = student.FullName;
            existingUser.UserName = student.UserName;
            existingUser.IsActive = student.IsActive;
            _userManager.Update(existingUser.Id, existingUser);
        }
    }

    public async Task UpdateAsync(Guid id, Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);

        var existingStudent = await _studentManager.GetByIdAsync(id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        await _studentManager.UpdateAsync(id, student);

        var existingUser = await _userManager.GetByIdAsync(student.UserId);
        if (existingUser != null)
        {
            existingUser.FullName = student.FullName;
            existingUser.UserName = student.UserName;
            existingUser.IsActive = student.IsActive;
            await _userManager.UpdateAsync(existingUser.Id, existingUser);
        }
    }

    private static void ValidateStudent(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        AccessValidator.ValidateUserName(student.UserName);
        AccessValidator.ValidateFullName(student.FullName);
        AccessValidator.ValidateStudentNumber(student.StudentNumber);
        AccessValidator.ValidateEmail(student.Email);
        AccessValidator.ValidatePhone(student.Phone);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();

        var student = _studentManager.GetById(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        _studentManager.Delete(id);
        _userManager.Delete(student.UserId);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();

        var student = await _studentManager.GetByIdAsync(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        await _studentManager.DeleteAsync(id);
        await _userManager.DeleteAsync(student.UserId);
    }

    public List<Student> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        return _studentManager.Search(regex);
    }

    public async Task<List<Student>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        return await _studentManager.SearchAsync(regex);
    }
}
