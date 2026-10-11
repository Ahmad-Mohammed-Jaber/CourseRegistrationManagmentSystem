using Mozaic.CourseRegistrationManagementSystem.BL.Managers;
using Mozaic.CourseRegistrationManagementSystem.BL.Validation;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Exceptions;
using Mozaic.CourseRegistrationManagementSystem.Shared.Logging;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Services;

public static class StudentRegistrationService
{
    private static async Task<int?> RequireStudentIdAsync()
    {
        var session = SessionManager.Current;
        if (session == null)
        {
            return null;
        }

        return await ResolveStudentIdAsync(session.UserId);
    }

    private static async Task<int?> ResolveStudentIdAsync(int userId)
    {
        var studentManager = new StudentManager();
        var student = await studentManager.GetByUserIdAsync(userId);
        var profile = StudentValidator.RequireProfileExists(student);
        if (!profile.IsSuccess)
        {
            return null;
        }

        return student!.Id;
    }

    public static async Task<int?> GetStudentIdForUserAsync(int userId)
    {
        return await ResolveStudentIdAsync(userId);
    }

    public static async Task<List<(Registration Registration, Class Class)>> GetRegistrationsForUserAsync(int userId)
    {
        try
        {
            var studentId = await ResolveStudentIdAsync(userId);
            if (studentId == null)
            {
                return new List<(Registration, Class)>();
            }

            return await GetRegistrationsForStudentAsync(studentId.Value);
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying GetRegistrationsForUserAsync()", ex);
        }
    }

    public static async Task DropRegistrationForUserAsync(int userId, int registrationId)
    {
        var studentId = await ResolveStudentIdAsync(userId);
        if (studentId == null)
        {
            throw new BusinessException("Student login required.");
        }

        await DropRegistrationForStudentAsync(studentId.Value, registrationId, userId);
    }

    public static async Task RegisterClassForUserAsync(int userId, int classId)
    {
        var studentId = await ResolveStudentIdAsync(userId);
        if (studentId == null)
        {
            throw new BusinessException("Student login required.");
        }

        await RegisterClassForStudentAsync(studentId.Value, classId, userId);
    }

    private static async Task<List<(Registration Registration, Class Class)>> GetRegistrationsForStudentAsync(int studentId)
    {
        var registrationManager = new RegistrationManager();
        return await registrationManager.GetStudentRegistrationsWithClassesAsync(studentId);
    }

    private static async Task RegisterClassForStudentAsync(int studentId, int classId, int? actorId = null)
    {
        try
        {
            var classManager = new ClassManager();
            var registrationManager = new RegistrationManager();

            Class? @class = await classManager.GetByIdAsync(classId);

            var exists = ClassValidator.RequireExists(@class, classId);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var active = ClassValidator.ValidateIsActive(@class!);
            if (!active.IsSuccess)
            {
                throw new BusinessException(active.Message);
            }

            var dup = RegistrationValidator.ValidateAlreadyRegistered(
                await registrationManager.ExistsAsync(studentId, classId));
            if (!dup.IsSuccess)
            {
                throw new BusinessException(dup.Message);
            }

            var capacity = ClassValidator.ValidateCapacityAvailable(@class!);
            if (!capacity.IsSuccess)
            {
                throw new BusinessException(capacity.Message);
            }

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? 0;
            var registration = new Registration
            {
                StudentId = studentId,
                ClassId = classId,
                RegistrationDate = DateTime.Now,
                Status = "Registered",
                CreatedBy = effectiveActorId,
                ModifiedBy = effectiveActorId
            };
            await registrationManager.AddAsync(registration);

            @class!.CurrentCapacity++;
            @class.ModifiedBy = effectiveActorId > 0 ? effectiveActorId : @class.ModifiedBy;
            await classManager.UpdateAsync(@class);
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterClass()", ex);
        }
    }

    private static async Task DropRegistrationForStudentAsync(int studentId, int registrationId, int? actorId = null)
    {
        try
        {
            var registrationManager = new RegistrationManager();
            var classManager = new ClassManager();

            Registration? registration = await registrationManager.GetByIdAsync(registrationId);

            var exists = RegistrationValidator.RequireExists(registration, registrationId);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var ownership = RegistrationValidator.ValidateOwnership(registration!.StudentId, studentId);
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

            var effectiveActorId = actorId.HasValue && actorId.Value > 0
                ? actorId.Value
                : SessionManager.Current?.UserId ?? @class.ModifiedBy;
            @class.ModifiedBy = effectiveActorId;
            await classManager.UpdateAsync(@class);
            return;
        }
        catch (BusinessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying DropRegistration()", ex);
        }
    }

    public static async Task RegisterClass(int classId)
    {
        try
        {
            var studentId = await RequireStudentIdAsync();
            if (studentId == null)
            {
                throw new BusinessException("Student login required.");
            }

            var classManager = new ClassManager();
            var registrationManager = new RegistrationManager();

            Class? @class = await classManager.GetByIdAsync(classId);

            var exists = ClassValidator.RequireExists(@class, classId);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
            }

            var active = ClassValidator.ValidateIsActive(@class!);
            if (!active.IsSuccess)
            {
                throw new BusinessException(active.Message);
            }

            var dup = RegistrationValidator.ValidateAlreadyRegistered(
                await registrationManager.ExistsAsync(studentId.Value, classId));
            if (!dup.IsSuccess)
            {
                throw new BusinessException(dup.Message);
            }

            var capacity = ClassValidator.ValidateCapacityAvailable(@class!);
            if (!capacity.IsSuccess)
            {
                throw new BusinessException(capacity.Message);
            }

            var sessionActorId = SessionManager.Current?.UserId ?? 0;
            var registration = new Registration
            {
                StudentId = studentId.Value,
                ClassId = classId,
                RegistrationDate = DateTime.Now,
                Status = "Registered",
                CreatedBy = sessionActorId,
                ModifiedBy = sessionActorId
            };
            await registrationManager.AddAsync(registration);

            @class!.CurrentCapacity++;
            @class.ModifiedBy = sessionActorId > 0 ? sessionActorId : @class.ModifiedBy;
            await classManager.UpdateAsync(@class);

            return;
        }
        catch (Exception ex)
        {
            AppLogger.LogCaught(ex);
            throw new BusinessException("An error occurred while trying RegisterClass()", ex);
        }
    }

    public static async Task DropRegistration(int registrationId)
    {
        try
        {
            var studentId = await RequireStudentIdAsync();
            if (studentId == null)
            {
                throw new BusinessException("Student login required.");
            }

            var registrationManager = new RegistrationManager();
            var classManager = new ClassManager();

            Registration? registration = await registrationManager.GetByIdAsync(registrationId);

            var exists = RegistrationValidator.RequireExists(registration, registrationId);
            if (!exists.IsSuccess)
            {
                throw new BusinessException(exists.Message);
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

            var dropActorId = SessionManager.Current?.UserId ?? @class.ModifiedBy;
            @class.ModifiedBy = dropActorId;
            await classManager.UpdateAsync(@class);
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
