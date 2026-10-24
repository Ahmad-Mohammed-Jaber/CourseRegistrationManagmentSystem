using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class StudentService
{
    /// <summary>
    /// The single read path for students. Every filter is optional; a null
    /// filter means "no restriction". The user's display columns and the
    /// audit actor names are joined in by the procedure, so no caller has to
    /// fetch the user row separately. Pass a page number to get one page
    /// plus the counter, or leave it out to get every matching row.
    /// </summary>
    public static async Task<StudentResult> GetAsync(
        int? id = null,
        int? userId = null,
        string? search = null,
        string? studentNumberFilter = null,
        string? fullNameFilter = null,
        string? userNameFilter = null,
        string? emailFilter = null,
        string? phoneFilter = null,
        bool? isActive = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        try
        {
            PagedResult<Student>.Normalize(ref pageNumber, ref pageSize);

            var term = string.IsNullOrWhiteSpace(search) ? null : search!.Trim();

            var studentManager = new StudentManager();
            var page = await studentManager.GetAsync(
                id, userId, term, studentNumberFilter, fullNameFilter, userNameFilter,
                emailFilter, phoneFilter, isActive, pageNumber, pageSize);

            // A page past the end of the result set comes back empty; fall back
            // to the last page so the view never shows "Page 9 of 3".
            if (page.Count == 0 && page.TotalPages > 0 && page.PageNumber > page.TotalPages)
            {
                page = await studentManager.GetAsync(
                    id, userId, term, studentNumberFilter, fullNameFilter, userNameFilter,
                    emailFilter, phoneFilter, isActive, page.TotalPages, pageSize);
            }

            return page;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving students.", ex);
        }
    }

    /// <summary>
    /// Fetches a single student by id, with the user's display columns
    /// joined in, or null when no such row exists.
    /// </summary>
    public static async Task<Student?> GetByIdAsync(int id)
    {
        try
        {
            var studentManager = new StudentManager();
            return await studentManager.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving student.", ex);
        }
    }

    public static async Task AddAsync(Student student, int? actorId = null)
    {
        try
        {
            var valid = StudentValidator.ValidateStudent(student);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            if (string.IsNullOrWhiteSpace(student.PasswordHash))
            {
                throw new BusinessException("Password is required for a new student.");
            }

            var unique = await EnsureUniqueUserNameAsync(student.UserName);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? 0;
            var userManager = new UserManager();
            var studentManager = new StudentManager();
            var user = new User
            {
                FullName = student.FullName,
                UserName = student.UserName,
                IsActive = student.IsActive,
                Role = User.UserRoles.Student,
                PasswordHash = student.PasswordHash,
                CreatedBy = effectiveActorId,
                ModifiedBy = effectiveActorId,
            };
            await userManager.AddAsync(user);

            var created = await userManager.GetByIdAsync(user.Id)
                ?? (await userManager.GetAsync(userName: user.UserName)).FirstOrDefault();
            if (created == null)
            {
                throw new BusinessException("Failed to create user account for student.");
            }

            student.UserId = created.Id;
            student.CreatedBy = effectiveActorId;
            student.ModifiedBy = effectiveActorId;

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
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding student.", ex);
        }
    }

    public static async Task UpdateAsync(Student student, int? actorId = null)
    {
        try
        {
            var valid = StudentValidator.ValidateStudent(student);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var existingStudent = await studentManager.GetByIdAsync(student.Id);
            var exists = StudentValidator.RequireExists(existingStudent, student.Id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var unique = await EnsureUniqueUserNameAsync(student.UserName, existingStudent!.UserId);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            student.CreatedBy = existingStudent!.CreatedBy;
            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? existingStudent.ModifiedBy;
            student.ModifiedBy = effectiveActorId;
            await studentManager.UpdateAsync(student);

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

                existingUser.ModifiedBy = effectiveActorId;
                await userManager.UpdateAsync(existingUser);
            }
            student.UserId = existingStudent.UserId;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating student.", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            var studentManager = new StudentManager();
            var userManager = new UserManager();
            var student = await studentManager.GetByIdAsync(id);
            var exists = StudentValidator.RequireExists(student, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            await studentManager.DeleteAsync(id);
            await userManager.DeleteAsync(student!.UserId);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting student.", ex);
        }
    }

    public static async Task<Student?> GetByUserIdAsync(int userId)
    {
        var page = await GetAsync(userId: userId);
        return page.FirstOrDefault();
    }

    public static async Task<Student?> GetCurrentStudentAsync()
    {
        var current = SessionManager.Current;
        if (current == null)
        {
            return null;
        }

        var page = await GetAsync(userId: current.UserId);
        return page.FirstOrDefault();
    }

    private static async Task<ValidationResult> EnsureUniqueUserNameAsync(string userName, int? excludeUserId = null)
    {
        var userManager = new UserManager();
        var existingUser = (await userManager.GetAsync(userName: userName)).FirstOrDefault();
        return UserValidator.RequireUniqueUserName(
            existingUser != null && existingUser.Id != (excludeUserId ?? 0), userName);
    }
}
