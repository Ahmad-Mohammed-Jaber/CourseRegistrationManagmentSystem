using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using System.Text.RegularExpressions;
using Shared.Logging;

namespace BL.Services;

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

    public static ValidationResult Add(Registration registration)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var student = EnsureStudentExistsSync(registration!.StudentId);
            if (!student.IsSuccess)
            {
                return student;
            }

            var @class = EnsureClassExistsSync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                return @class;
            }

            var registrationManager = new RegistrationManager();
            var dup = RegistrationValidator.ValidateNotDuplicate(
                registrationManager.GetRegistrationsByStudentId(registration.StudentId)
                    .Any(r => r.ClassId == registration.ClassId));
            if (!dup.IsSuccess)
            {
                return dup;
            }

            registrationManager.Add(registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public static async Task<ValidationResult> AddAsync(Registration registration)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var student = await EnsureStudentExistsAsync(registration!.StudentId);
            if (!student.IsSuccess)
            {
                return student;
            }

            var @class = await EnsureClassExistsAsync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                return @class;
            }

            var registrationManager = new RegistrationManager();
            var dup = RegistrationValidator.ValidateNotDuplicate(
                await registrationManager.ExistsAsync(registration.StudentId, registration.ClassId));
            if (!dup.IsSuccess)
            {
                return dup;
            }

            await registrationManager.AddAsync(registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public static ValidationResult Update(int id, Registration registration)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var registrationManager = new RegistrationManager();
            var existing = registrationManager.GetById(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var student = EnsureStudentExistsSync(registration!.StudentId);
            if (!student.IsSuccess)
            {
                return student;
            }

            var @class = EnsureClassExistsSync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                return @class;
            }

            if (existing!.StudentId != registration.StudentId || existing.ClassId != registration.ClassId)
            {
                var dup = RegistrationValidator.ValidateNotDuplicate(
                    registrationManager.GetRegistrationsByStudentId(registration.StudentId)
                        .Any(r => r.ClassId == registration.ClassId));
                if (!dup.IsSuccess)
                {
                    return dup;
                }
            }

            registrationManager.Update(id, registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
        }
    }

    public static async Task<ValidationResult> UpdateAsync(int id, Registration registration)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return auth;
            }

            var valid = RegistrationValidator.ValidateRegistration(registration);
            if (!valid.IsSuccess)
            {
                return valid;
            }

            var registrationManager = new RegistrationManager();
            var existing = await registrationManager.GetByIdAsync(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var student = await EnsureStudentExistsAsync(registration!.StudentId);
            if (!student.IsSuccess)
            {
                return student;
            }

            var @class = await EnsureClassExistsAsync(registration.ClassId);
            if (!@class.IsSuccess)
            {
                return @class;
            }

            if (existing!.StudentId != registration.StudentId || existing.ClassId != registration.ClassId)
            {
                var dup = RegistrationValidator.ValidateNotDuplicate(
                    await registrationManager.ExistsAsync(registration.StudentId, registration.ClassId));
                if (!dup.IsSuccess)
                {
                    return dup;
                }
            }

            await registrationManager.UpdateAsync(id, registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
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

            var registrationManager = new RegistrationManager();
            var existing = registrationManager.GetById(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            registrationManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting registration.", ex);
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

            var registrationManager = new RegistrationManager();
            var existing = await registrationManager.GetByIdAsync(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await registrationManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
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
