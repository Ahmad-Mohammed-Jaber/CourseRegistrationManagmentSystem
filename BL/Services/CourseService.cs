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

    public static Result<Course> Add(Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Course>.From(auth);
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return Result<Course>.From(valid);
            }

            var unique = EnsureUniqueCourseCodeSync(course.CourseCode);
            if (!unique.IsSuccess)
            {
                return Result<Course>.From(unique);
            }

            var courseManager = new CourseManager();
            int newId = courseManager.Add(course);
            if (newId <= 0)
            {
                return Result<Course>.Fail(ValidationStatus.Conflict, $"Failed to create course: database reported no new id.");
            }
            course.Id = newId;
            return Result<Course>.Ok(course);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public static async Task<Result<Course>> AddAsync(Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Course>.From(auth);
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return Result<Course>.From(valid);
            }

            var unique = await EnsureUniqueCourseCodeAsync(course.CourseCode);
            if (!unique.IsSuccess)
            {
                return Result<Course>.From(unique);
            }

            var courseManager = new CourseManager();
            int newId = await courseManager.AddAsync(course);
            if (newId <= 0)
            {
                return Result<Course>.Fail(ValidationStatus.Conflict, $"Failed to create course: database reported no new id.");
            }
            course.Id = newId;
            return Result<Course>.Ok(course);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public static Result<Course> Update(int id, Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Course>.From(auth);
            }

            var courseManager = new CourseManager();
            var existingCourse = courseManager.GetById(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return Result<Course>.From(exists);
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return Result<Course>.From(valid);
            }

            var unique = EnsureUniqueCourseCodeSync(course.CourseCode, id);
            if (!unique.IsSuccess)
            {
                return Result<Course>.From(unique);
            }

            int outcome = courseManager.Update(id, course);
            if (outcome <= 0)
            {
                return Result<Course>.Fail(ValidationStatus.NotFound, $"Course with id {id} not found.");
            }
            course.Id = id;
            return Result<Course>.Ok(course);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
    }

    public static async Task<Result<Course>> UpdateAsync(int id, Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Course>.From(auth);
            }

            var courseManager = new CourseManager();
            var existingCourse = await courseManager.GetByIdAsync(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return Result<Course>.From(exists);
            }

            var valid = CourseValidator.ValidateCourse(course);
            if (!valid.IsSuccess)
            {
                return Result<Course>.From(valid);
            }

            var unique = await EnsureUniqueCourseCodeAsync(course.CourseCode, id);
            if (!unique.IsSuccess)
            {
                return Result<Course>.From(unique);
            }

            int outcome = await courseManager.UpdateAsync(id, course);
            if (outcome <= 0)
            {
                return Result<Course>.Fail(ValidationStatus.NotFound, $"Course with id {id} not found.");
            }
            course.Id = id;
            return Result<Course>.Ok(course);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
    }

    public static Result<int> Delete(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<int>.From(auth);
            }

            var courseManager = new CourseManager();
            var existingCourse = courseManager.GetById(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return Result<int>.From(exists);
            }

            int outcome = courseManager.Delete(id);
            if (outcome <= 0)
            {
                return Result<int>.Fail(ValidationStatus.NotFound, $"Course with id {id} not found.");
            }
            return Result<int>.Ok(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting course.", ex);
        }
    }

    public static async Task<Result<int>> DeleteAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<int>.From(auth);
            }

            var courseManager = new CourseManager();
            var existingCourse = await courseManager.GetByIdAsync(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return Result<int>.From(exists);
            }

            int outcome = await courseManager.DeleteAsync(id);
            if (outcome <= 0)
            {
                return Result<int>.Fail(ValidationStatus.NotFound, $"Course with id {id} not found.");
            }
            return Result<int>.Ok(id);
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
