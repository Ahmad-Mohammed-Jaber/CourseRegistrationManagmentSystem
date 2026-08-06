using CourseRegistrationManagmentSystem.Business.Managers;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Shared.Session;

namespace CourseRegistrationManagmentSystem.Business.Services;
public class StudentRegistrationService
{
    private readonly ClassManager _classManager = new ClassManager();
    private readonly RegistrationManager _registrationManager = new RegistrationManager();

    public async Task RegisterClass(Guid classId)
    {
        AccessValidator.RequireRole(User.UserRoles.Student);
        Guid studentId = SessionManager.StudentSession!.Id;

        Class? @class = await _classManager.GetByIdAsync(classId);

        if (@class == null)
        {
            throw new KeyNotFoundException($"Class with id {classId} not found.");
        }

        if (!@class.IsActive)
        {
            throw new InvalidOperationException("This class is no longer active.");
        }

        if (await _registrationManager.ExistsAsync(studentId, classId))
        {
            throw new InvalidOperationException("Class already registered");
        }

        if (@class.CurrentCapacity >= @class.MaxCapacity)
        {
            throw new InvalidOperationException("Class is full");
        }

        var registration = new Registration
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            ClassId = classId,
            RegsitrationDate = DateTime.Now,
            Status = "Registered"
        };
        await _registrationManager.AddAsync(registration);

        @class.CurrentCapacity++;
        await _classManager.UpdateAsync(classId, @class);
    }

    public async Task DropRegistration(Guid registrationId)
    {
        AccessValidator.RequireRole(User.UserRoles.Student);
        Guid studentId = SessionManager.StudentSession!.Id;

        Registration? registration = await _registrationManager.GetByIdAsync(registrationId);

        if (registration == null)
        {
            throw new KeyNotFoundException($"Registration with id {registrationId} not found.");
        }

        if (registration.StudentId != studentId)
        {
            throw new UnauthorizedAccessException("You do not have permission to drop this registration.");
        }

        Class? @class = await _classManager.GetByIdAsync(registration.ClassId);

        if (@class == null)
        {
            throw new KeyNotFoundException($"Class with id {registration.ClassId} not found.");
        }

        await _registrationManager.DeleteAsync(registrationId);

        @class.CurrentCapacity--;
        if (@class.CurrentCapacity < 0) @class.CurrentCapacity = 0;
        await _classManager.UpdateAsync(@class.Id, @class);
    }
    public async Task<List<(Registration Registration, Class Class)>> GetRegistrationsAsync()
    {
        AccessValidator.RequireRole(User.UserRoles.Student);

        Guid studentId = SessionManager.StudentSession!.Id;

        return await _registrationManager
            .GetStudentRegistrationsWithClassesAsync(studentId);
    }
}
