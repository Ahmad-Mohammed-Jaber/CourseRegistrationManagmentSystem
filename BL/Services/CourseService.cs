using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Logging;

namespace BL.Services;

public class CourseService : ICrudService<Course>
{
    private readonly CourseManager _courseManager = new CourseManager();

    public Result<Course?> GetById(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<Course?>(auth.Status, auth.Errors, default);
            }

            return new Result<Course?>(ValidationStatus.Success, Array.Empty<ValidationError>(), _courseManager.GetById(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving course.", ex);
        }
    }

    public async Task<Result<Course?>> GetByIdAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<Course?>(auth.Status, auth.Errors, default);
            }

            return new Result<Course?>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _courseManager.GetByIdAsync(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving course.", ex);
        }
    }

    public Result<List<Course>> GetAll()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Course>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<Course>>(ValidationStatus.Success, Array.Empty<ValidationError>(), _courseManager.GetAll());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving courses.", ex);
        }
    }

    public async Task<Result<List<Course>>> GetAllAsync()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Course>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<Course>>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _courseManager.GetAllAsync());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving courses.", ex);
        }
    }

    public ValidationResult Add(Course course)
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

            _courseManager.Add(course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public async Task<ValidationResult> AddAsync(Course course)
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

            await _courseManager.AddAsync(course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding course.", ex);
        }
    }

    public ValidationResult Update(int id, Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingCourse = _courseManager.GetById(id);
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

            _courseManager.Update(id, course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
    }

    public async Task<ValidationResult> UpdateAsync(int id, Course course)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingCourse = await _courseManager.GetByIdAsync(id);
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

            await _courseManager.UpdateAsync(id, course);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating course.", ex);
        }
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

            var existingCourse = _courseManager.GetById(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            _courseManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting course.", ex);
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

            var existingCourse = await _courseManager.GetByIdAsync(id);
            var exists = CourseValidator.RequireExists(existingCourse, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await _courseManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting course.", ex);
        }
    }

    public Result<List<Course>> Search(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Course>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Course>>(pattern.Status, pattern.Errors, default);
            }

            return new Result<List<Course>>(ValidationStatus.Success, Array.Empty<ValidationError>(), _courseManager.Search(regex));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching courses.", ex);
        }
    }

    public async Task<Result<List<Course>>> SearchAsync(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Course>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Course>>(pattern.Status, pattern.Errors, default);
            }

            return new Result<List<Course>>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _courseManager.SearchAsync(regex));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching courses.", ex);
        }
    }

    private ValidationResult EnsureUniqueCourseCodeSync(string courseCode, int? excludeCourseId = null)
    {
        var existing = _courseManager.GetAll()
            .FirstOrDefault(c => c.CourseCode.Equals(courseCode.Trim(), StringComparison.OrdinalIgnoreCase));
        return CourseValidator.RequireUniqueCourseCode(
            existing != null && existing.Id != (excludeCourseId ?? 0), courseCode);
    }

    private async Task<ValidationResult> EnsureUniqueCourseCodeAsync(string courseCode, int? excludeCourseId = null)
    {
        var existing = (await _courseManager.GetAllAsync())
            .FirstOrDefault(c => c.CourseCode.Equals(courseCode.Trim(), StringComparison.OrdinalIgnoreCase));
        return CourseValidator.RequireUniqueCourseCode(
            existing != null && existing.Id != (excludeCourseId ?? 0), courseCode);
    }
}
