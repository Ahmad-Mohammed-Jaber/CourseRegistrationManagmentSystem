using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using System.Text.RegularExpressions;
using Shared.Logging;

namespace BL.Services;

public class RegistrationService : ICrudService<Registration>
{
    private readonly RegistrationManager _registrationManager = new RegistrationManager();
    private readonly StudentManager _studentManager = new StudentManager();
    private readonly ClassManager _classManager = new ClassManager();

    public Result<Registration?> GetById(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<Registration?>(auth.Status, auth.Errors, default);
            }

            return new Result<Registration?>(ValidationStatus.Success, Array.Empty<ValidationError>(), _registrationManager.GetById(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registration.", ex);
        }
    }

    public async Task<Result<Registration?>> GetByIdAsync(int id)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<Registration?>(auth.Status, auth.Errors, default);
            }

            return new Result<Registration?>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _registrationManager.GetByIdAsync(id));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registration.", ex);
        }
    }

    public Result<List<Registration>> GetAll()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Registration>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<Registration>>(ValidationStatus.Success, Array.Empty<ValidationError>(), _registrationManager.GetAll());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registrations.", ex);
        }
    }

    public async Task<Result<List<Registration>>> GetAllAsync()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Registration>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<Registration>>(ValidationStatus.Success, Array.Empty<ValidationError>(), await _registrationManager.GetAllAsync());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registrations.", ex);
        }
    }

    public ValidationResult Add(Registration registration)
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

            var dup = RegistrationValidator.ValidateNotDuplicate(
                _registrationManager.GetRegistrationsByStudentId(registration.StudentId)
                    .Any(r => r.ClassId == registration.ClassId));
            if (!dup.IsSuccess)
            {
                return dup;
            }

            _registrationManager.Add(registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public async Task<ValidationResult> AddAsync(Registration registration)
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

            var dup = RegistrationValidator.ValidateNotDuplicate(
                await _registrationManager.ExistsAsync(registration.StudentId, registration.ClassId));
            if (!dup.IsSuccess)
            {
                return dup;
            }

            await _registrationManager.AddAsync(registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while adding registration.", ex);
        }
    }

    public ValidationResult Update(int id, Registration registration)
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

            var existing = _registrationManager.GetById(id);
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
                    _registrationManager.GetRegistrationsByStudentId(registration.StudentId)
                        .Any(r => r.ClassId == registration.ClassId));
                if (!dup.IsSuccess)
                {
                    return dup;
                }
            }

            _registrationManager.Update(id, registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
        }
    }

    public async Task<ValidationResult> UpdateAsync(int id, Registration registration)
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

            var existing = await _registrationManager.GetByIdAsync(id);
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
                    await _registrationManager.ExistsAsync(registration.StudentId, registration.ClassId));
                if (!dup.IsSuccess)
                {
                    return dup;
                }
            }

            await _registrationManager.UpdateAsync(id, registration);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while updating registration.", ex);
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

            var existing = _registrationManager.GetById(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            _registrationManager.Delete(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting registration.", ex);
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

            var existing = await _registrationManager.GetByIdAsync(id);
            var exists = RegistrationValidator.RequireExists(existing, id);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            await _registrationManager.DeleteAsync(id);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while deleting registration.", ex);
        }
    }

    public Result<List<Registration>> Search(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Registration>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Registration>>(pattern.Status, pattern.Errors, default);
            }

            var registrations = _registrationManager.GetAll();
            return new Result<List<Registration>>(ValidationStatus.Success, Array.Empty<ValidationError>(), registrations
                .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
                .ToList());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching registrations.", ex);
        }
    }

    public async Task<Result<List<Registration>>> SearchAsync(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<Registration>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<Registration>>(pattern.Status, pattern.Errors, default);
            }

            var registrations = await _registrationManager.GetAllAsync();
            return new Result<List<Registration>>(ValidationStatus.Success, Array.Empty<ValidationError>(), registrations
                .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
                .ToList());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching registrations.", ex);
        }
    }

    public async Task<Result<List<(Registration Registration, Student Student, Class Class, Course Course)>>> GetAllDetailedAsync()
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<(Registration, Student, Class, Course)>>(auth.Status, auth.Errors, default);
            }

            return new Result<List<(Registration, Student, Class, Course)>>(
                ValidationStatus.Success,
                Array.Empty<ValidationError>(),
                await _registrationManager.GetAllDetailedAsync());
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while retrieving registration details.", ex);
        }
    }

    public async Task<Result<List<(Registration Registration, Student Student, Class Class, Course Course)>>> SearchDetailedAsync(string regex)
    {
        try
        {
            var auth = AccessValidator.RequireAdmin();
            if (!auth.IsSuccess)
            {
                return new Result<List<(Registration, Student, Class, Course)>>(auth.Status, auth.Errors, default);
            }

            var pattern = SearchValidator.ValidateSearchPattern(regex);
            if (!pattern.IsSuccess)
            {
                return new Result<List<(Registration, Student, Class, Course)>>(pattern.Status, pattern.Errors, default);
            }

            return new Result<List<(Registration, Student, Class, Course)>>(
                ValidationStatus.Success,
                Array.Empty<ValidationError>(),
                await _registrationManager.SearchDetailedAsync(regex));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while searching registration details.", ex);
        }
    }

    private ValidationResult EnsureStudentExistsSync(int studentId)
    {
        return StudentValidator.RequireExists(_studentManager.GetById(studentId), studentId);
    }

    private async Task<ValidationResult> EnsureStudentExistsAsync(int studentId)
    {
        return StudentValidator.RequireExists(await _studentManager.GetByIdAsync(studentId), studentId);
    }

    private ValidationResult EnsureClassExistsSync(int classId)
    {
        return ClassValidator.RequireExists(_classManager.GetById(classId), classId);
    }

    private async Task<ValidationResult> EnsureClassExistsAsync(int classId)
    {
        return ClassValidator.RequireExists(await _classManager.GetByIdAsync(classId), classId);
    }
}
