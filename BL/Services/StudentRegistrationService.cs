using BL.Managers;
using BL.Validation;
using Shared.Entities;
using Shared.Session;

namespace BL.Services;

public class StudentRegistrationService
{
    private readonly ClassManager _classManager = new ClassManager();
    private readonly RegistrationManager _registrationManager = new RegistrationManager();
    private readonly StudentManager _studentManager = new StudentManager();

    /// <summary>
    /// Resolves Student.Id from DB via current UserSession.UserId.
    /// No student data is cached in session — always fresh from DB.
    /// </summary>
    private async Task<int> RequireStudentIdAsync()
    {
        AccessValidator.RequireStudent();
        var session = SessionManager.Current!;
        var student = await _studentManager.GetByUserIdAsync(session.UserId);
        if (student == null)
            throw new InvalidOperationException("Student profile not found for current user. Please contact admin.");
        return student.Id;
    }

    public async Task RegisterClass(int classId)
    {
        int studentId = await RequireStudentIdAsync();

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
            StudentId = studentId,
            ClassId = classId,
            RegsitrationDate = DateTime.Now,
            Status = "Registered"
        };
        await _registrationManager.AddAsync(registration);

        @class.CurrentCapacity++;
        await _classManager.UpdateAsync(classId, @class);
    }

    public async Task DropRegistration(int registrationId)
    {
        int studentId = await RequireStudentIdAsync();

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
        int studentId = await RequireStudentIdAsync();

        return await _registrationManager
            .GetStudentRegistrationsWithClassesAsync(studentId);
    }
}
