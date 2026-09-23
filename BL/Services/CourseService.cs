using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;

namespace BL.Services;

public static class CourseService
{
    public static Course? GetById(int id)
    {
        try
        {
            var courseManager = new CourseManager();
            return courseManager.GetById(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving course.", ex);
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

    public static List<Course> GetAll()
    {
        try
        {
            var courseManager = new CourseManager();
            return courseManager.GetAll();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving courses.", ex);
        }
    }

    public static async Task<List<Course>> GetAllAsync()
    {
        try
        {
            var courseManager = new CourseManager();
            return await courseManager.GetAllAsync();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving courses.", ex);
        }
    }

    public static ValidationResult Add(Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = EnsureUniqueCourseCodeSync(course.CourseCode);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            var courseManager = new CourseManager();
            courseManager.Add(course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public static async Task<ValidationResult> AddAsync(Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = await EnsureUniqueCourseCodeAsync(course.CourseCode);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            var courseManager = new CourseManager();
            await courseManager.AddAsync(course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public static ValidationResult Update(int id, Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var courseManager = new CourseManager();
            var existingCourse = courseManager.GetById(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = EnsureUniqueCourseCodeSync(course.CourseCode, id);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            courseManager.Update(id, course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
    }

    public static async Task<ValidationResult> UpdateAsync(int id, Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var courseManager = new CourseManager();
            var existingCourse = await courseManager.GetByIdAsync(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var unique = await EnsureUniqueCourseCodeAsync(course.CourseCode, id);
            if (!unique.IsSuccess)
            {
                return unique;
            }

            await courseManager.UpdateAsync(id, course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
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

            var courseManager = new CourseManager();
            var existingCourse = courseManager.GetById(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            courseManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting course.", ex);
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

            var courseManager = new CourseManager();
            var existingCourse = await courseManager.GetByIdAsync(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await courseManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting course.", ex);
        }
    }

    public static List<Course> Search(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Course>();
            }

            var courseManager = new CourseManager();
            return courseManager.Search(regex);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching courses.", ex);
        }
    }

    public static async Task<List<Course>> SearchAsync(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Course>();
            }

            var courseManager = new CourseManager();
            return await courseManager.SearchAsync(regex);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching courses.", ex);
        }
    }

    private static ValidationResult EnsureUniqueCourseCodeSync(string courseCode, int? excludeCourseId = null)
    {
        var courseManager = new CourseManager();
        var existing = courseManager.GetAll()
            .FirstOrDefault(c => c.CourseCode.Equals(courseCode.Trim(), StringComparison.OrdinalIgnoreCase));
        return CourseValidator.RequireUniqueCourseCode(
            existing != null && existing.Id != (excludeCourseId ?? 0), courseCode);
    }

    private static async Task<ValidationResult> EnsureUniqueCourseCodeAsync(string courseCode, int? excludeCourseId = null)
    {
        var courseManager = new CourseManager();
        var existing = (await courseManager.GetAllAsync())
            .FirstOrDefault(c => c.CourseCode.Equals(courseCode.Trim(), StringComparison.OrdinalIgnoreCase));
        return CourseValidator.RequireUniqueCourseCode(
            existing != null && existing.Id != (excludeCourseId ?? 0), courseCode);
    }
}
