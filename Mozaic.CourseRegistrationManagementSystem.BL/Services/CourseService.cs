using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class CourseService
{
    public static async Task<CourseResult> GetAsync(
        int? id = null,
        string? courseCode = null,
        string? search = null,
        string? codeFilter = null,
        string? nameFilter = null,
        string? descriptionFilter = null,
        bool? isActive = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        try
        {
            PagedResult<Course>.Normalize(ref pageNumber, ref pageSize);

            var term = string.IsNullOrWhiteSpace(search) ? null : search!.Trim();

            var courseManager = new CourseManager();
            var page = await courseManager.GetAsync(
                id, courseCode, term, codeFilter, nameFilter, descriptionFilter, isActive, pageNumber, pageSize);

            // A page past the end of the result set comes back empty; fall back
            // to the last page so the view never shows "Page 9 of 3".
            if (page.Count == 0 && page.TotalPages > 0 && page.PageNumber > page.TotalPages)
            {
                page = await courseManager.GetAsync(
                    id, courseCode, term, codeFilter, nameFilter, descriptionFilter, isActive, page.TotalPages, pageSize);
            }

            return page;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving courses.", ex);
        }
    }

    public static async Task<Course?> GetByIdAsync(int id)
    {
        try
        {
            var courseManager = new CourseManager();
            return await courseManager.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving course.", ex);
        }
    }

    public static async Task AddAsync(Course course, int? actorId = null)
    {
        try
        {
            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var unique = await EnsureUniqueCourseCodeAsync(course.CourseCode);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? 0;
            course.CreatedBy = effectiveActorId;
            course.ModifiedBy = effectiveActorId;
            var courseManager = new CourseManager();
            await courseManager.AddAsync(course);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public static async Task UpdateAsync(Course course, int? actorId = null)
    {
        try
        {
            var courseManager = new CourseManager();
            var existingCourse = await courseManager.GetByIdAsync(course.Id);
            var exists = CourseValidator.RequireExists(existingCourse, course.Id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var unique = await EnsureUniqueCourseCodeAsync(course.CourseCode, course.Id);
            if (!unique.IsSuccess)
            {
                throw new BusinessException(unique.Message);
            }

            course.CreatedBy = existingCourse!.CreatedBy;
            course.ModifiedBy = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? existingCourse.ModifiedBy;
            await courseManager.UpdateAsync(course);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
    }

    public static async Task DeleteAsync(int id)
    {
        try
        {
            var courseManager = new CourseManager();
            var existingCourse = await courseManager.GetByIdAsync(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            await courseManager.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting course.", ex);
        }
    }

    private static async Task<ValidationResult> EnsureUniqueCourseCodeAsync(string courseCode, int? excludeCourseId = null)
    {
        var courseManager = new CourseManager();
        var courses = await courseManager.GetAsync();
        var exists = courses.Any(c => c.Id != (excludeCourseId ?? 0)
            && c.IsActive
            && c.CourseCode.Equals(courseCode.Trim(), StringComparison.OrdinalIgnoreCase));
        return CourseValidator.RequireUniqueCourseCode(exists, courseCode);
    }
}
