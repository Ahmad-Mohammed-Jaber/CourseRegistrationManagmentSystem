using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using System.Text.RegularExpressions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

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

    public static void Add(Class classEntity)
    {
        try
        {
            var valid = ClassValidator.ValidateClass(classEntity);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var course = EnsureCourseExistsSync(classEntity.CourseId);
            if (!course.IsSuccess)
            {
                throw new BusinessException(course.Message);
            }

            var classManager = new ClassManager();
            classManager.Add(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static async Task AddAsync(Class classEntity)
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

            var classManager = new ClassManager();
            await classManager.AddAsync(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding class.", ex);
        }
    }

    public static void Update(Class classEntity)
    {
        try
        {
            var classManager = new ClassManager();
            var existingClass = classManager.GetById(classEntity.Id);
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

            var course = EnsureCourseExistsSync(classEntity.CourseId);
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

            classManager.Update(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
        }
    }

    public static async Task UpdateAsync(Class classEntity)
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

            await classManager.UpdateAsync(classEntity);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating class.", ex);
        }
    }

    public static void Delete(int id)
    {
        try
        {
            var classManager = new ClassManager();
            var existingClass = classManager.GetById(id);
            var exists = ClassValidator.RequireExists(existingClass, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            classManager.Delete(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting class.", ex);
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
