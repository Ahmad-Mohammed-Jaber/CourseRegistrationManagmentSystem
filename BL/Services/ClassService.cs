using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using System.Text.RegularExpressions;
using Shared.Logging;

namespace BL.Services;

public class ClassService : ICrudService<Class>
{
    private readonly ClassManager _classManager = new ClassManager();
    private readonly CourseManager _courseManager = new CourseManager();

    public Result<Class?> GetById(int id)
    {
        try
        {
            var auth = AccessValidator.RequireLogin();
            if (!auth.IsSuccess)
            {
                return new Result<Class?>(auth.Status, auth.Errors, default);
            }

            return new Result<Class?>(ValidationStatus.Success, Array.Empty<ValidationError>(), _classManager.GetById(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving class.", ex);
        }
    }

    public async Task<Result<Class?>> GetByIdAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireLogin();
            if (!auth.IsSuccess)
            {
                return new Result<Class?>(auth.Status, auth.Errors, default);
            }

            return new Result<Class?>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _classManager.GetByIdAsync(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving class.", ex);
        }
    }

    public Result<List<Class>> GetAll()
    {
        try
        {
            var auth = AccessValidator.RequireLogin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Class>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<Class>>(ValidationStatus.Success, Array.Empty<ValidationError>(), _classManager.GetAll());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving classes.", ex);
        }
    }

    public async Task<Result<List<Class>>> GetAllAsync()
    {
        try
        {
            var auth = AccessValidator.RequireLogin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Class>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<Class>>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _classManager.GetAllAsync());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving classes.", ex);
        }
    }

    public ValidationResult Add(Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var course = EnsureCourseExistsSync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return course;
            }

            _classManager.Add(classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public async Task<ValidationResult> AddAsync(Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var course = await EnsureCourseExistsAsync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return course;
            }

            await _classManager.AddAsync(classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public ValidationResult Update(int id, Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingClass = _classManager.GetById(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var course = EnsureCourseExistsSync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return course;
            }

            var capacity = ClassValidator.ValidateMaxCapacityNotBelowEnrollment(
                classEntity.MaxCapacity, existingClass!.CurrentCapacity);
            if (!capacity.IsSuccess)
            {
                return capacity;
            }

            _classManager.Update(id, classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
        }
    }

    public async Task<ValidationResult> UpdateAsync(int id, Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var existingClass = await _classManager.GetByIdAsync(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var course = await EnsureCourseExistsAsync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return course;
            }

            var capacity = ClassValidator.ValidateMaxCapacityNotBelowEnrollment(
                classEntity.MaxCapacity, existingClass!.CurrentCapacity);
            if (!capacity.IsSuccess)
            {
                return capacity;
            }

            await _classManager.UpdateAsync(id, classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
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

            var existingClass = _classManager.GetById(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            _classManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
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

            var existingClass = await _classManager.GetByIdAsync(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await _classManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
        }
    }

    public Result<List<Class>> Search(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireLogin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Class>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Class>>(pattern.Status, pattern.Errors, default);
            }

            var classes = _classManager.GetAll();
            return new Result<List<Class>>(ValidationStatus.Success, Array.Empty<ValidationError>(), classes
                .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                                      Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
                .ToList());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching classes.", ex);
        }
    }

    public async Task<Result<List<Class>>> SearchAsync(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireLogin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Class>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Class>>(pattern.Status, pattern.Errors, default);
            }

            var classes = await _classManager.GetAllAsync();
            return new Result<List<Class>>(ValidationStatus.Success, Array.Empty<ValidationError>(), classes
                .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                                      Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
                .ToList());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching classes.", ex);
        }
    }

    private ValidationResult EnsureCourseExistsSync(int courseId)
    {
        return CourseValidator.RequireExists(_courseManager.GetById(courseId), courseId);
    }

    private async Task<ValidationResult> EnsureCourseExistsAsync(int courseId)
    {
        return CourseValidator.RequireExists(await _courseManager.GetByIdAsync(courseId), courseId);
    }
}
