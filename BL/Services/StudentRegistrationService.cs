using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Exceptions;
using Shared.Session;
using Shared.Logging;

namespace BL.Services;

public static class StudentRegistrationService
{
    private static async Task<int?> RequireStudentIdAsync()
    {
        var auth = AccessValidator.RequireStudent();
        if (!auth.IsSuccess)
        {
            return null;
        }

        var session = SessionManager.Current;
        if (session == null)
        {
            return null;
        }

        var studentManager = new StudentManager();
        var student = await studentManager.GetByUserIdAsync(session.UserId);
        var profile = StudentValidator.RequireProfileExists(student);
        if (!profile.IsSuccess)
        {
            return null;
        }

        return student!.Id;
    }

    public static async Task<int?> RegisterClass(int classId)
    {
        try
        {
            var studentId = await RequireStudentIdAsync();
            if (studentId == null)
            {
                return null;
            }

            var classManager = new ClassManager();
            var registrationManager = new RegistrationManager();

            Class? @class = await classManager.GetByIdAsync(classId);

            var exists = ClassValidator.RequireExists(@class, classId);
            if (!exists.IsSuccess)
            {
                return null;
            }

            var active = ClassValidator.ValidateIsActive(@class!);
            if (!active.IsSuccess)
            {
                return null;
            }

            var dup = RegistrationValidator.ValidateAlreadyRegistered(
                await registrationManager.ExistsAsync(studentId.Value, classId));
            if (!dup.IsSuccess)
            {
                return null;
            }

            var capacity = ClassValidator.ValidateCapacityAvailable(@class!);
            if (!capacity.IsSuccess)
            {
                return null;
            }

            var registration = new Registration
            {
                StudentId = studentId.Value,
                ClassId = classId,
                RegistrationDate = DateTime.Now,
                Status = "Registered"
            };
            await registrationManager.AddAsync(registration);

            @class!.CurrentCapacity++;
            await classManager.UpdateAsync(classId, @class);

            return registration.Id;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterClass()", ex);
        }
    }

    public static async Task<ValidationResult> DropRegistration(int registrationId)
    {
        try
        {
            var studentId = await RequireStudentIdAsync();
            if (studentId == null)
            {
                return new ValidationResult(
                    ValidationStatus.Unauthorized,
                    new[] { new ValidationError(string.Empty, "Student login required.") });
            }

            var registrationManager = new RegistrationManager();
            var classManager = new ClassManager();

            Registration? registration = await registrationManager.GetByIdAsync(registrationId);

            var exists = RegistrationValidator.RequireExists(registration, registrationId);
            if (!exists.IsSuccess)
            {
                return exists;
            }

            var ownership = RegistrationValidator.ValidateOwnership(registration!.StudentId, studentId.Value);
            if (!ownership.IsSuccess)
            {
                throw new BusinessException(ownership.Message);
            }

            Class? @class = await classManager.GetByIdAsync(registration.ClassId);

            if (@class == null)
            {
                throw new BusinessException($"Class with id {registration.ClassId} not found.");
            }

            await registrationManager.DeleteAsync(registrationId);

            @class.CurrentCapacity--;
            if (@class.CurrentCapacity < 0)
            {
                @class.CurrentCapacity = 0;
            }

            await classManager.UpdateAsync(@class.Id, @class);
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

    public static async Task<List<(Registration Registration, Class Class)>> GetRegistrationsAsync()
    {
        try
        {
            var studentId = await RequireStudentIdAsync();
            if (studentId == null)
            {
                return new List<(Registration, Class)>();
            }

            var registrationManager = new RegistrationManager();
            return await registrationManager.GetStudentRegistrationsWithClassesAsync(studentId.Value);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying GetRegistrationsAsync()", ex);
        }
    }
}
