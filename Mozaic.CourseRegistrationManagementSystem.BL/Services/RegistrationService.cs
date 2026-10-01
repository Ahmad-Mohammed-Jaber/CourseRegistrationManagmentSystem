using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using System.Text.RegularExpressions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class RegistrationService
{
    public static Registration? GetById(int id)
    {
        try
        {
            var registrationManager = new RegistrationManager();
            return registrationManager.GetById(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registration.", ex);
        }
    }

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

    public static List<Registration> GetAll()
    {
        try
        {
            var registrationManager = new RegistrationManager();
            return registrationManager.GetAll();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registrations.", ex);
        }
    }

    public static async Task<List<Registration>> GetAllAsync()
    {
        try
        {
            var registrationManager = new RegistrationManager();
            return await registrationManager.GetAllAsync();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registrations.", ex);
        }
    }

    public static void Add(Registration registration)
    {
        try
        {
            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var student = EnsureStudentExistsSync(registration!.StudentId);
            if (!student.IsSuccess)
            {
                throw new BusinessException(student.Message);
            }

            var @class = EnsureClassExistsSync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                throw new BusinessException(@class.Message);
            }

            var registrationManager = new RegistrationManager();
            var dup = RegistrationValidator.ValidateNotDuplicate(
                registrationManager.GetRegistrationsByStudentId(registration.StudentId)
                    .Any(r => r.ClassId == registration.ClassId));
            if (!dup.IsSuccess)
            {
                throw new BusinessException(dup.Message);
            }

            registrationManager.Add(registration);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public static async Task AddAsync(Registration registration)
    {
        try
        {
            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var student = await EnsureStudentExistsAsync(registration!.StudentId);
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
                await registrationManager.ExistsAsync(registration.StudentId, registration.ClassId));
            if (!dup.IsSuccess)
            {
                throw new BusinessException(dup.Message);
            }

            await registrationManager.AddAsync(registration);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public static void Update(Registration registration)
    {
        try
        {
            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                throw new BusinessException(valid.Message);
            }

            var registrationManager = new RegistrationManager();
            var existing = registrationManager.GetById(registration.Id);
            var exists = RegistrationValidator.RequireExists(existing, registration.Id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var student = EnsureStudentExistsSync(registration!.StudentId);
            if (!student.IsSuccess)
            {
                throw new BusinessException(student.Message);
            }

            var @class = EnsureClassExistsSync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                throw new BusinessException(@class.Message);
            }

            if (existing!.StudentId != registration.StudentId || existing.ClassId != registration.ClassId)
            {
                var dup = RegistrationValidator.ValidateNotDuplicate(
                    registrationManager.GetRegistrationsByStudentId(registration.StudentId)
                        .Any(r => r.ClassId == registration.ClassId));
                if (!dup.IsSuccess)
                {
                    throw new BusinessException(dup.Message);
                }
            }

            registrationManager.Update(registration);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
        }
    }

    public static async Task UpdateAsync(Registration registration)
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

            var student = await EnsureStudentExistsAsync(registration!.StudentId);
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
                    await registrationManager.ExistsAsync(registration.StudentId, registration.ClassId));
                if (!dup.IsSuccess)
                {
                    throw new BusinessException(dup.Message);
                }
            }

            await registrationManager.UpdateAsync(registration);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
        }
    }

    public static void Delete(int id)
    {
        try
        {
            var registrationManager = new RegistrationManager();
            var existing = registrationManager.GetById(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            registrationManager.Delete(id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting registration.", ex);
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

    public static List<Registration> Search(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Registration>();
            }

            var registrationManager = new RegistrationManager();
            var registrations = registrationManager.GetAll();
            return registrations
                .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching registrations.", ex);
        }
    }

    public static async Task<List<Registration>> SearchAsync(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<Registration>();
            }

            var registrationManager = new RegistrationManager();
            var registrations = await registrationManager.GetAllAsync();
            return registrations
                .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching registrations.", ex);
        }
    }

    public static async Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> GetAllDetailedAsync()
    {
        try
        {
            var registrationManager = new RegistrationManager();
            return await registrationManager.GetAllDetailedAsync();
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registration details.", ex);
        }
    }

    public static async Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> SearchDetailedAsync(string regex)
    {
        try
        {
            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new List<(Registration, Student, Class, Course)>();
            }

            var registrationManager = new RegistrationManager();
            return await registrationManager.SearchDetailedAsync(regex);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching registration details.", ex);
        }
    }

    private static ValidationResult EnsureStudentExistsSync(int studentId)
    {
        var studentManager = new StudentManager();
        return StudentValidator.RequireExists(studentManager.GetById(studentId), studentId);
    }

    private static async Task<ValidationResult> EnsureStudentExistsAsync(int studentId)
    {
        var studentManager = new StudentManager();
        return StudentValidator.RequireExists(await studentManager.GetByIdAsync(studentId), studentId);
    }

    private static ValidationResult EnsureClassExistsSync(int classId)
    {
        var classManager = new ClassManager();
        return ClassValidator.RequireExists(classManager.GetById(classId), classId);
    }

    private static async Task<ValidationResult> EnsureClassExistsAsync(int classId)
    {
        var classManager = new ClassManager();
        return ClassValidator.RequireExists(await classManager.GetByIdAsync(classId), classId);
    }
}
