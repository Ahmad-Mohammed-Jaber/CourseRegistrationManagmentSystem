using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class ClassService
{
    /// <summary>
    /// The single read path for classes. Every filter is optional; a null
    /// filter means "no restriction". Pass a page number to get one page
    /// plus the counters, or leave it out to get every matching row.
    /// </summary>
    public static async Task<ClassResult> GetAsync(
        int? id = null,
        int? courseId = null,
        int? userId = null,
        string? search = null,
        string? statusFilter = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        try
        {
            PagedResult<Class>.Normalize(ref pageNumber, ref pageSize);

            var term = string.IsNullOrWhiteSpace(search) ? null : search!.Trim();
            var status = NormalizeStatusFilter(statusFilter);

            var classManager = new ClassManager();
            var page = await classManager.GetAsync(id, courseId, userId, term, status, pageNumber, pageSize);

            // A page past the end of the result set comes back empty; fall back
            // to the last page so the view never shows "Page 9 of 3".
            if (page.Count == 0 && page.TotalPages > 0 && page.PageNumber > page.TotalPages)
            {
                page = await classManager.GetAsync(id, courseId, userId, term, status, page.TotalPages, pageSize);
            }

            return page;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving classes.", ex);
        }
    }

    /// <summary>
    /// Fetches a single class by id, or null when no such row exists.
    /// </summary>
    public static async Task<Class?> GetByIdAsync(int id)
    {
        try
        {
            var classManager = new ClassManager();
            return await classManager.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving class.", ex);
        }
    }

    public static async Task AddAsync(Class classEntity, int? actorId = null)
    {
        try
        {
            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var course = await EnsureCourseExistsAsync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                throw new BusinessException(course.Message);
            }

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? 0;
            classEntity.CreatedBy = effectiveActorId;
            classEntity.ModifiedBy = effectiveActorId;
            var classManager = new ClassManager();
            await classManager.AddAsync(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static async Task UpdateAsync(Class classEntity, int? actorId = null)
    {
        try
        {
            var classManager = new ClassManager();
            var existingClass = await classManager.GetByIdAsync(classEntity.Id);
            var exists = ClassValidator.RequireExists(existingClass, classEntity.Id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var course = await EnsureCourseExistsAsync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                throw new BusinessException(course.Message);
            }

            var capacity = ClassValidator.ValidateMaxCapacityNotBelowEnrollment(
                classEntity.MaxCapacity, existingClass!.CurrentCapacity);
            if (!capacity.IsSuccess)
            {
                throw new BusinessException(capacity.Message);
            }

            classEntity.CreatedBy = existingClass!.CreatedBy;
            classEntity.ModifiedBy = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? existingClass.ModifiedBy;
            await classManager.UpdateAsync(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            var classManager = new ClassManager();
            var existingClass = await classManager.GetByIdAsync(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            await classManager.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
        }
    }

    private static async Task<ValidationResult> EnsureCourseExistsAsync(int courseId)
    {
        var courseManager = new CourseManager();
        var course = await courseManager.GetByIdAsync(courseId);
        return CourseValidator.RequireExists(course, courseId);
    }

    /// <summary>
    /// Maps the requested status filter onto the values the procedure
    /// understands. A null filter means "no restriction" (every class);
    /// an unrecognised value falls back to the active classes.
    /// </summary>
    private static string? NormalizeStatusFilter(string? statusFilter)
    {
        if (string.IsNullOrWhiteSpace(statusFilter))
        {
            return null;
        }

        if (string.Equals(statusFilter, ClassResult.StatusInactive, StringComparison.OrdinalIgnoreCase))
        {
            return ClassResult.StatusInactive;
        }

        if (string.Equals(statusFilter, ClassResult.StatusAll, StringComparison.OrdinalIgnoreCase))
        {
            return ClassResult.StatusAll;
        }

        return ClassResult.StatusActive;
    }
}
