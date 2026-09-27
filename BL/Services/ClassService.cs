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

    public static Result<Class> Add(Class classEntity)
    {
        try
        {
            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return Result<Class>.From(valid);
            }

            var course = EnsureCourseExistsSync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return Result<Class>.From(course);
            }

            var classManager = new ClassManager();
            int newId = classManager.Add(classEntity);
            if (newId <= 0)
            {
                return Result<Class>.Fail(ValidationStatus.Conflict, $"Failed to create class: database reported no new id.");
            }
            classEntity.Id = newId;
            return Result<Class>.Ok(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static async Task<Result<Class>> AddAsync(Class classEntity)
    {
        try
        {
            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return Result<Class>.From(valid);
            }

            var course = await EnsureCourseExistsAsync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return Result<Class>.From(course);
            }

            var classManager = new ClassManager();
            int newId = await classManager.AddAsync(classEntity);
            if (newId <= 0)
            {
                return Result<Class>.Fail(ValidationStatus.Conflict, $"Failed to create class: database reported no new id.");
            }
            classEntity.Id = newId;
            return Result<Class>.Ok(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static Result<Class> Update(int id, Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Class>.From(auth);
            }

            var classManager = new ClassManager();
            var existingClass = classManager.GetById(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return Result<Class>.From(exists);
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return Result<Class>.From(valid);
            }

            var course = EnsureCourseExistsSync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return Result<Class>.From(course);
            }

            var capacity = ClassValidator.ValidateMaxCapacityNotBelowEnrollment(
                classEntity.MaxCapacity, existingClass!.CurrentCapacity);
            if (!capacity.IsSuccess)
            {
                return Result<Class>.From(capacity);
            }

            int outcome = classManager.Update(id, classEntity);
            if (outcome <= 0)
            {
                return Result<Class>.Fail(ValidationStatus.NotFound, $"Class with id {id} not found.");
            }
            classEntity.Id = id;
            return Result<Class>.Ok(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
        }
    }

    public static async Task<Result<Class>> UpdateAsync(int id, Class classEntity)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return Result<Class>.From(auth);
            }

            var classManager = new ClassManager();
            var existingClass = await classManager.GetByIdAsync(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return Result<Class>.From(exists);
            }

            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                return Result<Class>.From(valid);
            }

            var course = await EnsureCourseExistsAsync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                return Result<Class>.From(course);
            }

            var capacity = ClassValidator.ValidateMaxCapacityNotBelowEnrollment(
                classEntity.MaxCapacity, existingClass!.CurrentCapacity);
            if (!capacity.IsSuccess)
            {
                return Result<Class>.From(capacity);
            }

            int outcome = await classManager.UpdateAsync(id, classEntity);
            if (outcome <= 0)
            {
                return Result<Class>.Fail(ValidationStatus.NotFound, $"Class with id {id} not found.");
            }
            classEntity.Id = id;
            return Result<Class>.Ok(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
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

            var classManager = new ClassManager();
            var existingClass = classManager.GetById(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return Result<int>.From(exists);
            }

            int outcome = classManager.Delete(id);
            if (outcome <= 0)
            {
                return Result<int>.Fail(ValidationStatus.NotFound, $"Class with id {id} not found.");
            }
            return Result<int>.Ok(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
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

            var classManager = new ClassManager();
            var existingClass = await classManager.GetByIdAsync(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                return Result<int>.From(exists);
            }

            int outcome = await classManager.DeleteAsync(id);
            if (outcome <= 0)
            {
                return Result<int>.Fail(ValidationStatus.NotFound, $"Class with id {id} not found.");
            }
            return Result<int>.Ok(id);
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
