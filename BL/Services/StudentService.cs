using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.DTOs;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;

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

    public Result<Student?> GetById(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<Student?>(auth.Status, auth.Errors, default);
            }

            var student = _studentManager.GetById(id);
            if (student != null)
            {
                EnrichSync(student);
            }

            return new Result<Student?>(ValidationStatus.Success, Array.Empty<ValidationError>(), student);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student.", ex);
        }
    }

    public async Task<Result<Student?>> GetByIdAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<Student?>(auth.Status, auth.Errors, default);
            }

            var student = await _studentManager.GetByIdAsync(id);
            if (student != null)
            {
                await EnrichAsync(student);
            }

            return new Result<Student?>(ValidationStatus.Success, Array.Empty<ValidationError>(), student);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student.", ex);
        }
    }

    public Result<List<Student>> GetAll()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Student>>(auth.Status, auth.Errors, default);
            }

            var list = _studentManager.GetAll();
            foreach (var s in list)
            {
                EnrichSync(s);
            }

            return new Result<List<Student>>(ValidationStatus.Success, Array.Empty<ValidationError>(), list);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving students.", ex);
        }
    }

    public async Task<Result<List<Student>>> GetAllAsync()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Student>>(auth.Status, auth.Errors, default);
            }

            var list = await _studentManager.GetAllAsync();
            foreach (var s in list)
            {
                await EnrichAsync(s);
            }

            return new Result<List<Student>>(ValidationStatus.Success, Array.Empty<ValidationError>(), list);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving students.", ex);
        }
    }

    public ValidationResult Add(Student student)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = StudentValidator.ValidateStudent(student);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            if (student != null && string.IsNullOrWhiteSpace(student.PasswordHash))
            {
                return new ValidationResult(
                    ValidationStatus.Invalid,
                    new[] { new ValidationError(nameof(Student.PasswordHash), "Password is required for a new student.") });
            }

            var unique = EnsureUniqueUserNameSync(student!.UserName);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            var user = new User
            {
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
            };

            _userManager.Add(user);
            if (user.Id == 0)
            {
                var created = _userManager.GetAll().FirstOrDefault(u => u.UserName == student.UserName);
                if (created != null)
                {
                    user.Id = created.Id;
                }
            }
            if (user.Id == 0)
            {
                throw new BusinessException("Failed to create user account for student.");
            }

            student.UserId = user.Id;

            try
            {
                _studentManager.Add(student);
            }
            catch
            {
                try
                {
                    _userManager.Delete(user.Id);
                }
                catch (Exception ex)
                {
                    AppLogger.LogCaught(ex);
                }

                throw;
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding student.", ex);
        }
    }

    public async Task<ValidationResult> AddAsync(Student student)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = StudentValidator.ValidateStudent(student);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            if (student != null && string.IsNullOrWhiteSpace(student.PasswordHash))
            {
                return new ValidationResult(
                    ValidationStatus.Invalid,
                    new[] { new ValidationError(nameof(Student.PasswordHash), "Password is required for a new student.") });
            }

            var unique = await EnsureUniqueUserNameAsync(student!.UserName);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            var user = new User
            {
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
            };
            await _userManager.AddAsync(user);
            var created = await _userManager.GetByUserNameAsync(user.UserName);
            if (created == null)
            {
                throw new BusinessException("Failed to create user account for student.");
            }

            student.UserId = created.Id;

            try
            {
                await _studentManager.AddAsync(student);
            }
            catch
            {
                try
                {
                    await _userManager.DeleteAsync(created.Id);
                }
                catch (Exception ex)
                {
                    AppLogger.LogCaught(ex);
                }

                throw;
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding student.", ex);
        }
    }

    public ValidationResult Update(int id, Student student)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = StudentValidator.ValidateStudent(student);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var existingStudent = _studentManager.GetById(id);
            var exists = StudentValidator.RequireExists(existingStudent, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var userByName = _userManager.GetAll().FirstOrDefault(u => u.UserName == student!.UserName);
            var unique = UserValidator.RequireUniqueUserName(
                userByName != null && userByName.Id != existingStudent!.UserId, student.UserName);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            _studentManager.Update(id, student);

            var existingUser = _userManager.GetById(existingStudent.UserId);
            if (existingUser != null)
            {
                existingUser.FullName = student.FullName;
                existingUser.UserName = student.UserName;
                existingUser.IsActive = student.IsActive;
                if (!string.IsNullOrWhiteSpace(student.PasswordHash))
                {
                    existingUser.PasswordHash = student.PasswordHash;
                }

                _userManager.Update(existingUser.Id, existingUser);
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating student.", ex);
        }
    }

    public async Task<ValidationResult> UpdateAsync(int id, Student student)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = StudentValidator.ValidateStudent(student);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var existingStudent = await _studentManager.GetByIdAsync(id);
            var exists = StudentValidator.RequireExists(existingStudent, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var unique = await EnsureUniqueUserNameAsync(student!.UserName, existingStudent!.UserId);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            await _studentManager.UpdateAsync(id, student);

            var existingUser = await _userManager.GetByIdAsync(existingStudent.UserId);
            if (existingUser != null)
            {
                existingUser.FullName = student.FullName;
                existingUser.UserName = student.UserName;
                existingUser.IsActive = student.IsActive;
                if (!string.IsNullOrWhiteSpace(student.PasswordHash))
                {
                    existingUser.PasswordHash = student.PasswordHash;
                }

                await _userManager.UpdateAsync(existingUser.Id, existingUser);
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating student.", ex);
        }
    }

    private ValidationResult EnsureUniqueUserNameSync(string userName)
    {
        var existing = _userManager.GetAll().FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        return UserValidator.RequireUniqueUserName(existing != null, userName);
    }

    private async Task<ValidationResult> EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var existingUser = await _userManager.GetByUserNameAsync(userName);
        return UserValidator.RequireUniqueUserName(
            existingUser != null && existingUser.Id != (excludeUserId ?? 0), userName);
    }

    public ValidationResult Delete(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var student = _studentManager.GetById(id);
            var exists = StudentValidator.RequireExists(student, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            _studentManager.Delete(id);
            _userManager.Delete(student!.UserId);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting student.", ex);
        }
    }

    public async Task<ValidationResult> DeleteAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var student = await _studentManager.GetByIdAsync(id);
            var exists = StudentValidator.RequireExists(student, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await _studentManager.DeleteAsync(id);
            await _userManager.DeleteAsync(student!.UserId);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting student.", ex);
        }
    }

    public Result<List<Student>> Search(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Student>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Student>>(pattern.Status, pattern.Errors, default);
            }

            var result = _studentManager.Search(regex);
            foreach (var s in result)
            {
                EnrichSync(s);
            }

            return new Result<List<Student>>(ValidationStatus.Success, Array.Empty<ValidationError>(), result);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching students.", ex);
        }
    }

    public async Task<Result<List<Student>>> SearchAsync(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Student>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Student>>(pattern.Status, pattern.Errors, default);
            }

            var result = await _studentManager.SearchAsync(regex);
            foreach (var s in result)
            {
                await EnrichAsync(s);
            }

            return new Result<List<Student>>(ValidationStatus.Success, Array.Empty<ValidationError>(), result);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching students.", ex);
        }
    }


    public async Task<Result<Student?>> GetByUserIdAsync(int userId)
    {
        try
        {
            var auth = AccessValidator.RequireOwnerOrAdmin(userId);
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

            var student = await _studentManager.GetByUserIdAsync(userId);
            if (student != null)
            {
                await EnrichAsync(student);
            }

            return new Result<Student?>(ValidationStatus.Success, Array.Empty<ValidationError>(), student);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student profile.", ex);
        }
    }

    public async Task<Result<Student?>> GetCurrentStudentAsync()
    {
        try
        {
            var auth = AccessValidator.RequireStudent();
            if (!auth.IsSuccess)
            {
                return new Result<Student?>(auth.Status, auth.Errors, default);
            }

            var current = Shared.Session.SessionManager.Current!;
            var student = await _studentManager.GetByUserIdAsync(current.UserId);
            if (student != null)
            {
                await EnrichAsync(student);
            }

            return new Result<Student?>(ValidationStatus.Success, Array.Empty<ValidationError>(), student);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving current student profile.", ex);
        }
    }


    public async Task<Result<StudentProfile?>> GetProfileByUserIdAsync(int userId)
    {
        try
        {
            var auth = AccessValidator.RequireOwnerOrAdmin(userId);
            if (!auth.IsSuccess)
            {
                throw new BusinessException(auth.Message);
            }

            return new Result<StudentProfile?>(
                ValidationStatus.Success,
                Array.Empty<ValidationError>(),
                await _studentManager.GetProfileByUserIdAsync(userId));
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student profile.", ex);
        }
    }

    public async Task<Result<StudentProfile?>> GetCurrentProfileAsync()
    {
        try
        {
            var auth = AccessValidator.RequireStudent();
            if (!auth.IsSuccess)
            {
                return new Result<StudentProfile?>(auth.Status, auth.Errors, default);
            }

            var current = Shared.Session.SessionManager.Current!;
            return new Result<StudentProfile?>(
                ValidationStatus.Success,
                Array.Empty<ValidationError>(),
                await _studentManager.GetProfileByUserIdAsync(current.UserId));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving current student profile.", ex);
        }
    }
}
