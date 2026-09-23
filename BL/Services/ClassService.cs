using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using System.Text.RegularExpressions;
using Shared.Logging;

namespace BL.Services;

public static class ClassService
{
    public static Class? GetById(int id)
    {
        try
        {
            var classManager = new ClassManager();
            return classManager.GetById(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving class.", ex);
        }
    }

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

    public static List<Class> GetAll()
    {
        try
        {
            var classManager = new ClassManager();
            return classManager.GetAll();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving classes.", ex);
        }
    }

    public static async Task<List<Class>> GetAllAsync()
    {
        try
        {
            var classManager = new ClassManager();
            return await classManager.GetAllAsync();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving classes.", ex);
        }
    }

    public static ValidationResult Add(Class classEntity)
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

            var classManager = new ClassManager();
            classManager.Add(classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static async Task<ValidationResult> AddAsync(Class classEntity)
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

            var classManager = new ClassManager();
            await classManager.AddAsync(classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static ValidationResult Update(int id, Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var classManager = new ClassManager();
            var existingClass = classManager.GetById(id);
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

            classManager.Update(id, classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
        }
    }

    public static async Task<ValidationResult> UpdateAsync(int id, Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var classManager = new ClassManager();
            var existingClass = await classManager.GetByIdAsync(id);
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

            await classManager.UpdateAsync(id, classEntity);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
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

            var classManager = new ClassManager();
            var existingClass = classManager.GetById(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            classManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
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

            var classManager = new ClassManager();
            var existingClass = await classManager.GetByIdAsync(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await classManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
        }
    }

    public static List<Class> Search(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Class>();
            }

            var classManager = new ClassManager();
            var classes = classManager.GetAll();
            return classes
                .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                                      Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching classes.", ex);
        }
    }

    public static async Task<List<Class>> SearchAsync(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Class>();
            }

            var classManager = new ClassManager();
            var classes = await classManager.GetAllAsync();
            return classes
                .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                                      Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching classes.", ex);
        }
    }

    private static ValidationResult EnsureCourseExistsSync(int courseId)
    {
        var courseManager = new CourseManager();
        return CourseValidator.RequireExists(courseManager.GetById(courseId), courseId);
    }

    private static async Task<ValidationResult> EnsureCourseExistsAsync(int courseId)
    {
        var courseManager = new CourseManager();
        return CourseValidator.RequireExists(await courseManager.GetByIdAsync(courseId), courseId);
    }
}
