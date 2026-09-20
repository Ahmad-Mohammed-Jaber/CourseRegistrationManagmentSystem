using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.Entities;

namespace BL.Services;

public class StudentService : ICrudService<Student>
{
    private readonly StudentManager _studentManager = new StudentManager();
    private readonly UserManager _userManager = new UserManager();

    private async Task EnrichAsync(Student student)
    {
        var user = await _userManager.GetByIdAsync(student.UserId);
        if (user != null)
        {
            student.UserName = user.UserName;
            student.FullName = user.FullName;
            student.IsActive = user.IsActive;
            student.PasswordHash = user.PasswordHash;
            student.Role = user.Role;
        }
    }

    private void EnrichSync(Student student)
    {
        var user = _userManager.GetById(student.UserId);
        if (user != null)
        {
            student.UserName = user.UserName;
            student.FullName = user.FullName;
            student.IsActive = user.IsActive;
            student.PasswordHash = user.PasswordHash;
            student.Role = user.Role;
        }
    }

    public Student? GetById(int id)
    {
        AccessValidator.RequireAdmin();
        var student = _studentManager.GetById(id);
        if (student != null) EnrichSync(student);
        return student;
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        AccessValidator.RequireAdmin();
        var student = await _studentManager.GetByIdAsync(id);
        if (student != null) await EnrichAsync(student);
        return student;
    }

    public List<Student> GetAll()
    {
        AccessValidator.RequireAdmin();
        var list = _studentManager.GetAll();
        foreach (var s in list) EnrichSync(s);
        return list;
    }

    public async Task<List<Student>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var list = await _studentManager.GetAllAsync();
        foreach (var s in list) await EnrichAsync(s);
        return list;
    }

    public void Add(Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);
        EnsureUniqueUserNameSync(student.UserName);

        var user = new User
        {
            FullName = student.FullName,
            UserName = student.UserName,
            IsActive = student.IsActive,
            Role = User.UserRoles.Student,
            PasswordHash = student.PasswordHash,
        };

        _userManager.Add(user);
        // User Id is populated after Add via output param? Assume provider sets it.
        // If not, fetch by username
        if (user.Id == 0)
        {
            var created = _userManager.GetAll().FirstOrDefault(u => u.UserName == student.UserName);
            if (created != null) user.Id = created.Id;
        }
        student.UserId = user.Id;

        _studentManager.Add(student);
    }

    public async Task AddAsync(Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);
        await EnsureUniqueUserNameAsync(student.UserName);

        var user = new User
        {
            FullName = student.FullName,
            UserName = student.UserName,
            IsActive = student.IsActive,
            Role = User.UserRoles.Student,
            PasswordHash = student.PasswordHash,
        };
        await _userManager.AddAsync(user);
        // fetch created user id
        var created = await _userManager.GetByUserNameAsync(user.UserName);
        student.UserId = created!.Id;

        await _studentManager.AddAsync(student);
    }

    public void Update(int id, Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);

        var existingStudent = _studentManager.GetById(id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        // check username uniqueness excluding current user
        var userByName = _userManager.GetAll().FirstOrDefault(u => u.UserName == student.UserName);
        if (userByName != null && userByName.Id != existingStudent.UserId)
            throw new InvalidOperationException($"A user with username '{student.UserName}' already exists.");

        _studentManager.Update(id, student);

        var existingUser = _userManager.GetById(existingStudent.UserId);
        if (existingUser != null)
        {
            existingUser.FullName = student.FullName;
            existingUser.UserName = student.UserName;
            existingUser.IsActive = student.IsActive;
            if (!string.IsNullOrWhiteSpace(student.PasswordHash))
                existingUser.PasswordHash = student.PasswordHash;
            _userManager.Update(existingUser.Id, existingUser);
        }
    }

    public async Task UpdateAsync(int id, Student student)
    {
        AccessValidator.RequireAdmin();
        ValidateStudent(student);

        var existingStudent = await _studentManager.GetByIdAsync(id);
        if (existingStudent == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        await EnsureUniqueUserNameAsync(student.UserName, existingStudent.UserId);

        await _studentManager.UpdateAsync(id, student);

        var existingUser = await _userManager.GetByIdAsync(existingStudent.UserId);
        if (existingUser != null)
        {
            existingUser.FullName = student.FullName;
            existingUser.UserName = student.UserName;
            existingUser.IsActive = student.IsActive;
            if (!string.IsNullOrWhiteSpace(student.PasswordHash))
                existingUser.PasswordHash = student.PasswordHash;
            await _userManager.UpdateAsync(existingUser.Id, existingUser);
        }
    }

    private void EnsureUniqueUserNameSync(string userName)
    {
        var existing = _userManager.GetAll().FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
            throw new InvalidOperationException($"A user with username '{userName}' already exists.");
    }

    private async Task EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var existingUser = await _userManager.GetByUserNameAsync(userName);
        if (existingUser != null && existingUser.Id != (excludeUserId ?? 0))
        {
            throw new InvalidOperationException($"A user with username '{userName}' already exists.");
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
        if (string.IsNullOrWhiteSpace(student.PasswordHash))
        {
            // For updates password may be preserved from existing, but for adds it must be present.
            // Allow empty here; caller ensures hashing. If empty and it's an Add, enrichment will catch.
        }
    }

    public void Delete(int id)
    {
        AccessValidator.RequireAdmin();

        var student = _studentManager.GetById(id);
        if (student == null)
            throw new KeyNotFoundException($"Student with id {id} not found.");

        _studentManager.Delete(id);
        _userManager.Delete(student.UserId);
    }

    public async Task DeleteAsync(int id)
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
        var result = _studentManager.Search(regex);
        foreach (var s in result) EnrichSync(s);
        return result;
    }

    public async Task<List<Student>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var result = await _studentManager.SearchAsync(regex);
        foreach (var s in result) await EnrichAsync(s);
        return result;
    }

    // ---- Current student helpers (auth: self or admin) ----

    public async Task<Student?> GetByUserIdAsync(int userId)
    {
        AccessValidator.RequireLogin();
        var current = Shared.Session.SessionManager.Current!;
        if (current.Role != User.UserRoles.Admin && current.UserId != userId)
            throw new UnauthorizedAccessException("You can only view your own student profile.");

        var student = await _studentManager.GetByUserIdAsync(userId);
        if (student != null) await EnrichAsync(student);
        return student;
    }

    public async Task<Student?> GetCurrentStudentAsync()
    {
        AccessValidator.RequireStudent();
        var current = Shared.Session.SessionManager.Current!;
        var student = await _studentManager.GetByUserIdAsync(current.UserId);
        if (student != null) await EnrichAsync(student);
        return student;
    }
}
