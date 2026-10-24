using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class RegistrationService
{
    /// <summary>
    /// The single read path for registrations. Every filter is optional; a
    /// null filter means "no restriction". Each row carries the joined class,
    /// student and course, so no caller has to fetch them separately. Pass a
    /// page number to get one page plus the counter, or leave it out to get
    /// every matching row.
    /// </summary>
    public static async Task<RegistrationResult> GetAsync(
        int? id = null,
        int? studentId = null,
        int? classId = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        try
        {
            PagedResult<Registration>.Normalize(ref pageNumber, ref pageSize);

            var term = string.IsNullOrWhiteSpace(search) ? null : search!.Trim();

            var registrationManager = new RegistrationManager();
            var page = await registrationManager.GetAsync(id, studentId, classId, term, pageNumber, pageSize);

            // A page past the end of the result set comes back empty; fall back
            // to the last page so the view never shows "Page 9 of 3".
            if (page.Count == 0 && page.TotalPages > 0 && page.PageNumber > page.TotalPages)
            {
                page = await registrationManager.GetAsync(id, studentId, classId, term, page.TotalPages, pageSize);
            }

            return page;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registrations.", ex);
        }
    }

    /// <summary>
    /// Fetches a single registration by id, with the class, student and
    /// course joined in, or null when no such row exists.
    /// </summary>
    public static async Task<Registration?> GetByIdAsync(int id)
    {
        try
        {
            var registrationManager = new RegistrationManager();
            return await registrationManager.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registration.", ex);
        }
    }

    public static async Task AddAsync(Registration registration, int? actorId = null)
    {
        try
        {
            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var student = await EnsureStudentExistsAsync(registration.StudentId);
            if (!student.IsSuccess)
            {
                throw new BusinessException(student.Message);
            }

            var @class = await EnsureClassExistsAsync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                throw new BusinessException(@class.Message);
            }

            var registrationManager = new RegistrationManager();
            var dup = RegistrationValidator.ValidateNotDuplicate(
                (await registrationManager.GetAsync(studentId: registration.StudentId, classId: registration.ClassId))
                    .Items.Any());
            if (!dup.IsSuccess)
            {
                throw new BusinessException(dup.Message);
            }

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? 0;
            registration.CreatedBy = effectiveActorId;
            registration.ModifiedBy = effectiveActorId;
            await registrationManager.AddAsync(registration);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public static async Task UpdateAsync(Registration registration, int? actorId = null)
    {
        try
        {
            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var registrationManager = new RegistrationManager();
            var existing = await registrationManager.GetByIdAsync(registration.Id);
            var exists = RegistrationValidator.RequireExists(existing, registration.Id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var student = await EnsureStudentExistsAsync(registration.StudentId);
            if (!student.IsSuccess)
            {
                throw new BusinessException(student.Message);
            }

            var @class = await EnsureClassExistsAsync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                throw new BusinessException(@class.Message);
            }

            if (existing!.StudentId != registration.StudentId || existing.ClassId != registration.ClassId)
            {
                var dup = RegistrationValidator.ValidateNotDuplicate(
                    (await registrationManager.GetAsync(studentId: registration.StudentId, classId: registration.ClassId))
                        .Items.Any());
                if (!dup.IsSuccess)
                {
                    throw new BusinessException(dup.Message);
                }
            }

            registration.CreatedBy = existing!.CreatedBy;
            registration.ModifiedBy = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? existing.ModifiedBy;
            await registrationManager.UpdateAsync(registration);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            var registrationManager = new RegistrationManager();
            var existing = await registrationManager.GetByIdAsync(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            await registrationManager.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting registration.", ex);
        }
    }

    private static async Task<ValidationResult> EnsureStudentExistsAsync(int studentId)
    {
        var studentManager = new StudentManager();
        var student = await studentManager.GetByIdAsync(studentId);
        return StudentValidator.RequireExists(student, studentId);
    }

    private static async Task<ValidationResult> EnsureClassExistsAsync(int classId)
    {
        var classManager = new ClassManager();
        var @class = await classManager.GetByIdAsync(classId);
        return ClassValidator.RequireExists(@class, classId);
    }
}
