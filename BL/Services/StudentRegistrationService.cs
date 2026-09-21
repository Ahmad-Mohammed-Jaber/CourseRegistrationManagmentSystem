using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Session;
using Shared.Logging;

namespace BL.Services;

public class StudentRegistrationService
{
    private readonly ClassManager _classManager = new ClassManager();
    private readonly RegistrationManager _registrationManager = new RegistrationManager();
    private readonly StudentManager _studentManager = new StudentManager();

    private async Task<Result<int>> RequireStudentIdAsync()
    {
        var auth = AccessValidator.RequireStudent();
        if (!auth.IsSuccess)
        {
            return new Result<int>(auth.Status, auth.Errors, default);
        }

        var session = SessionManager.Current!;
        var student = await _studentManager.GetByUserIdAsync(session.UserId);
        var profile = StudentValidator.RequireProfileExists(student);
        if (!profile.IsSuccess)
        {
            return new Result<int>(profile.Status, profile.Errors, default);
        }

        return new Result<int>(ValidationStatus.Success, Array.Empty<ValidationError>(), student!.Id);
    }

    public async Task<Result<int>> RegisterClass(int classId)
    {
        try
        {
            var studentIdResult = await RequireStudentIdAsync();
            if (!studentIdResult.IsSuccess)
            {
                return studentIdResult;
            }

            int studentId = studentIdResult.Value;

            Class? @class = await _classManager.GetByIdAsync(classId);

            var exists = ClassValidator.RequireExists(@class, classId);
            if (!exists.IsSuccess)
            {
                return new Result<int>(exists.Status, exists.Errors, default);
            }

            var active = ClassValidator.ValidateIsActive(@class!);
            if (!active.IsSuccess)
            {
                return new Result<int>(active.Status, active.Errors, default);
            }

            var dup = RegistrationValidator.ValidateAlreadyRegistered(
                await _registrationManager.ExistsAsync(studentId, classId));
            if (!dup.IsSuccess)
            {
                return new Result<int>(dup.Status, dup.Errors, default);
            }

            var capacity = ClassValidator.ValidateCapacityAvailable(@class!);
            if (!capacity.IsSuccess)
            {
                return new Result<int>(capacity.Status, capacity.Errors, default);
            }

            var registration = new Registration
            {
                StudentId = studentId,
                ClassId = classId,
                RegistrationDate = DateTime.Now,
                Status = "Registered"
            };
            await _registrationManager.AddAsync(registration);

            @class!.CurrentCapacity++;
            await _classManager.UpdateAsync(classId, @class);

            return new Result<int>(ValidationStatus.Success, Array.Empty<ValidationError>(), registration.Id);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterClass()", ex);
        }
    }

    public async Task<ValidationResult> DropRegistration(int registrationId)
    {
        try
        {
            var studentIdResult = await RequireStudentIdAsync();
            if (!studentIdResult.IsSuccess)
            {
                return studentIdResult;
            }

            int studentId = studentIdResult.Value;

            Registration? registration = await _registrationManager.GetByIdAsync(registrationId);

            var exists = RegistrationValidator.RequireExists(registration, registrationId);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var ownership = RegistrationValidator.ValidateOwnership(registration!.StudentId, studentId);
            if (!ownership.IsSuccess)
            {
                throw new BusinessException(ownership.Message);
            }

            Class? @class = await _classManager.GetByIdAsync(registration.ClassId);

            if (@class == null)
            {
                throw new BusinessException($"Class with id {registration.ClassId} not found.");
            }

            await _registrationManager.DeleteAsync(registrationId);

            @class.CurrentCapacity--;
            if (@class.CurrentCapacity < 0)
            {
                @class.CurrentCapacity = 0;
            }

            await _classManager.UpdateAsync(@class.Id, @class);
            return new ValidationResult(ValidationStatus.Success, Array.Empty<ValidationError>());
        }
        catch (BusinessException ex)
        {
            AppLogger.LogCaught(ex);
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying DropRegistration()", ex);
        }
    }

    public async Task<Result<List<(Registration Registration, Class Class)>>> GetRegistrationsAsync()
    {
        try
        {
            var studentIdResult = await RequireStudentIdAsync();
            if (!studentIdResult.IsSuccess)
            {
                return new Result<List<(Registration, Class)>>(studentIdResult.Status, studentIdResult.Errors, default);
            }

            int studentId = studentIdResult.Value;

            return new Result<List<(Registration, Class)>>(
                ValidationStatus.Success,
                Array.Empty<ValidationError>(),
                await _registrationManager.GetStudentRegistrationsWithClassesAsync(studentId));
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying GetRegistrationsAsync()", ex);
        }
    }
}
