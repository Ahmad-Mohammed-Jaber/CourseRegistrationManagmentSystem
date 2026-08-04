using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Services;
public class StudentRegistrationService
{
    private readonly ClassRepository _classRepository = new ClassRepository();
    private readonly RegistrationRepository _registrationRepository = new RegistrationRepository();

    public async Task RegisterClass(Guid classId)
    {
        AccessValidator.RequireRole(User.UserRoles.Student);
        Guid studentId = SessionManager.StudentSession!.Id;

        Class? @class = await _classRepository.GetByIdAsync(classId);

        if (@class == null)
        {
            throw new KeyNotFoundException($"Class with id {classId} not found.");
        }

        if (!@class.IsActive)
        {
            throw new InvalidOperationException("This class is no longer active.");
        }

        if (await _registrationRepository.ExistsAsync(studentId, classId))
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
        await _registrationRepository.AddAsync(registration);

        @class.CurrentCapacity++;
        await _classRepository.UpdateAsync(classId, @class);
    }

    public async Task DropRegistration(Guid registrationId)
    {
        AccessValidator.RequireRole(User.UserRoles.Student);
        Guid studentId = SessionManager.StudentSession!.Id;

        Registration? registration = await _registrationRepository.GetByIdAsync(registrationId);

        if (registration == null)
        {
            throw new KeyNotFoundException($"Registration with id {registrationId} not found.");
        }

        if (registration.StudentId != studentId)
        {
            throw new UnauthorizedAccessException("You do not have permission to drop this registration.");
        }

        Class? @class = await _classRepository.GetByIdAsync(registration.ClassId);

        if (@class == null)
        {
            throw new KeyNotFoundException($"Class with id {registration.ClassId} not found.");
        }

        await _registrationRepository.DeleteAsync(registrationId);

        @class.CurrentCapacity--;
        if (@class.CurrentCapacity < 0) @class.CurrentCapacity = 0;
        await _classRepository.UpdateAsync(@class.Id, @class);
    }
    public async Task<List<RegistrationDetailsDto>> GetRegistrationsAsync()
    {
        AccessValidator.RequireRole(User.UserRoles.Student);

        Guid studentId = SessionManager.StudentSession!.Id;

        var registrations = await _registrationRepository
            .GetStudentRegistrationsWithClassesAsync(studentId);

        return registrations
            .Select(x => x.Registration.ToDetailsDto(x.Class))
            .ToList();
    }
}
