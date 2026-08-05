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

        // Business Logic / Validation
        if (student.UserId == Guid.Empty)
        {
            // In the previous implementation, the service created the user.
            // We should maintain that logic but use the UserManager.
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Unknown", // Need to handle how we get these from Student if not using DTOs
                UserName = "Unknown",
                IsActive = true,
                Role = User.UserRoles.Student,
                PasswordHash = "TemporaryPassword123!", // Should be handled by a password service/manager
            };
            _userManager.Add(user);
            student.UserId = user.Id;
        }

        _studentManager.Add(student);
    }

    public async Task AddAsync(Student student)
    {
        AccessValidator.RequireAdmin();

        if (student.UserId == Guid.Empty)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                FullName = "Unknown",
                UserName = "Unknown",
                IsActive = true,
                Role = User.UserRoles.Student,
                PasswordHash = "TemporaryPassword123!",
            };
            await _userManager.AddAsync(user);
            student.UserId = user.Id;
        }

        await _studentManager.AddAsync(student);
    }

    public void Update(Guid id, Student student)
    {
        AccessValidator.RequireAdmin();

        var existingStudent = _studentManager.GetById(id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        // Update student details
        _studentManager.Update(id, student);

        // If the student's associated user needs updating, we'd do it here via _userManager.
        // Since we are removing DTOs, we assume the caller provides the entity.
    }

    public async Task UpdateAsync(Guid id, Student student)
    {
        AccessValidator.RequireAdmin();

        var existingStudent = await _studentManager.GetByIdAsync(id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        await _studentManager.UpdateAsync(id, student);
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
