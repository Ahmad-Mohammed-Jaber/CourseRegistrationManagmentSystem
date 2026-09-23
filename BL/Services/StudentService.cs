using BL.Managers;
using BL.Validation;
using Shared.DTOs;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;

namespace BL.Services;

public static class StudentService
{
    private static async Task EnrichAsync(Student student, UserManager userManager)
    {
        var user = await userManager.GetByIdAsync(student.UserId);
        if (user != null)
        {
            student.UserName = user.UserName;
            student.FullName = user.FullName;
            student.IsActive = user.IsActive;
            student.PasswordHash = user.PasswordHash;
            student.Role = user.Role;
        }
    }

    private static void EnrichSync(Student student, UserManager userManager)
    {
        var user = userManager.GetById(student.UserId);
        if (user != null)
        {
            student.UserName = user.UserName;
            student.FullName = user.FullName;
            student.IsActive = user.IsActive;
            student.PasswordHash = user.PasswordHash;
            student.Role = user.Role;
        }
    }

    public static Student? GetById(int id)
    {
        try
        {
            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = studentManager.GetById(id);
            if (student != null)
            {
                EnrichSync(student, userManager);
            }

            return student;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student.", ex);
        }
    }

    public static async Task<Student?> GetByIdAsync(int id)
    {
        try
        {
            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = await studentManager.GetByIdAsync(id);
            if (student != null)
            {
                await EnrichAsync(student, userManager);
            }

            return student;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student.", ex);
        }
    }

    public static List<Student> GetAll()
    {
        try
        {
            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var list = studentManager.GetAll();
            foreach (var s in list)
            {
                EnrichSync(s, userManager);
            }

            return list;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving students.", ex);
        }
    }

    public static async Task<List<Student>> GetAllAsync()
    {
        try
        {
            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var list = await studentManager.GetAllAsync();
            foreach (var s in list)
            {
                await EnrichAsync(s, userManager);
            }

            return list;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving students.", ex);
        }
    }

    public static ValidationResult Add(Student student)
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

            var userManager = new UserManager();
            var studentManager = new StudentManager();
            var user = new User
            {
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
            };

            userManager.Add(user);
            if (user.Id == 0)
            {
                var created = userManager.GetAll().FirstOrDefault(u => u.UserName == student.UserName);
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
                studentManager.Add(student);
            }
            catch
            {
                try
                {
                    userManager.Delete(user.Id);
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

    public static async Task<ValidationResult> AddAsync(Student student)
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

            var userManager = new UserManager();
            var studentManager = new StudentManager();
            var user = new User
            {
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
            };
            await userManager.AddAsync(user);
            var created = await userManager.GetByUserNameAsync(user.UserName);
            if (created == null)
            {
                throw new BusinessException("Failed to create user account for student.");
            }

            student.UserId = created.Id;

            try
            {
                await studentManager.AddAsync(student);
            }
            catch
            {
                try
                {
                    await userManager.DeleteAsync(created.Id);
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

    public static ValidationResult Update(int id, Student student)
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

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var existingStudent = studentManager.GetById(id);
            var exists = StudentValidator.RequireExists(existingStudent, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var userByName = userManager.GetAll().FirstOrDefault(u => u.UserName == student!.UserName);
            var unique = UserValidator.RequireUniqueUserName(
                userByName != null && userByName.Id != existingStudent!.UserId, student.UserName);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            studentManager.Update(id, student);

            var existingUser = userManager.GetById(existingStudent.UserId);
            if (existingUser != null)
            {
                existingUser.FullName = student.FullName;
                existingUser.UserName = student.UserName;
                existingUser.IsActive = student.IsActive;
                if (!string.IsNullOrWhiteSpace(student.PasswordHash))
                {
                    existingUser.PasswordHash = student.PasswordHash;
                }

                userManager.Update(existingUser.Id, existingUser);
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating student.", ex);
        }
    }

    public static async Task<ValidationResult> UpdateAsync(int id, Student student)
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

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var existingStudent = await studentManager.GetByIdAsync(id);
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

            await studentManager.UpdateAsync(id, student);

            var existingUser = await userManager.GetByIdAsync(existingStudent.UserId);
            if (existingUser != null)
            {
                existingUser.FullName = student.FullName;
                existingUser.UserName = student.UserName;
                existingUser.IsActive = student.IsActive;
                if (!string.IsNullOrWhiteSpace(student.PasswordHash))
                {
                    existingUser.PasswordHash = student.PasswordHash;
                }

                await userManager.UpdateAsync(existingUser.Id, existingUser);
            }
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating student.", ex);
        }
    }

    private static ValidationResult EnsureUniqueUserNameSync(string userName)
    {
        var userManager = new UserManager();
        var existing = userManager.GetAll().FirstOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        return UserValidator.RequireUniqueUserName(existing != null, userName);
    }

    private static async Task<ValidationResult> EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var userManager = new UserManager();
        var existingUser = await userManager.GetByUserNameAsync(userName);
        return UserValidator.RequireUniqueUserName(
            existingUser != null && existingUser.Id != (excludeUserId ?? 0), userName);
    }

    public static ValidationResult Delete(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = studentManager.GetById(id);
            var exists = StudentValidator.RequireExists(student, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            studentManager.Delete(id);
            userManager.Delete(student!.UserId);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting student.", ex);
        }
    }

    public static async Task<ValidationResult> DeleteAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = await studentManager.GetByIdAsync(id);
            var exists = StudentValidator.RequireExists(student, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await studentManager.DeleteAsync(id);
            await userManager.DeleteAsync(student!.UserId);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting student.", ex);
        }
    }

    public static List<Student> Search(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Student>();
            }

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var result = studentManager.Search(regex);
            foreach (var s in result)
            {
                EnrichSync(s, userManager);
            }

            return result;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching students.", ex);
        }
    }

    public static async Task<List<Student>> SearchAsync(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Student>();
            }

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var result = await studentManager.SearchAsync(regex);
            foreach (var s in result)
            {
                await EnrichAsync(s, userManager);
            }

            return result;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching students.", ex);
        }
    }


    public static async Task<Student?> GetByUserIdAsync(int userId)
    {
        try
        {
            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = await studentManager.GetByUserIdAsync(userId);
            if (student != null)
            {
                await EnrichAsync(student, userManager);
            }

            return student;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student profile.", ex);
        }
    }

    public static async Task<Student?> GetCurrentStudentAsync()
    {
        try
        {
            var current = Shared.Session.SessionManager.Current;
            if (current == null)
            {
                return null;
            }

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = await studentManager.GetByUserIdAsync(current.UserId);
            if (student != null)
            {
                await EnrichAsync(student, userManager);
            }

            return student;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving current student profile.", ex);
        }
    }


    public static async Task<StudentProfile?> GetProfileByUserIdAsync(int userId)
    {
        try
        {
            var studentManager = new StudentManager();
            return await studentManager.GetProfileByUserIdAsync(userId);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student profile.", ex);
        }
    }

    public static async Task<StudentProfile?> GetCurrentProfileAsync()
    {
        try
        {
            var current = Shared.Session.SessionManager.Current;
            if (current == null)
            {
                return null;
            }

            var studentManager = new StudentManager();
            return await studentManager.GetProfileByUserIdAsync(current.UserId);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving current student profile.", ex);
        }
    }
}
