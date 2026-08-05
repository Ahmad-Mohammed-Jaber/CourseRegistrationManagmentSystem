using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Managers;

internal class RegistrationManager
{
    private readonly RegistrationRepository _registrationRepository = new RegistrationRepository();

    public Registration? GetById(Guid id) => _registrationRepository.GetById(id);

    public Task<Registration?> GetByIdAsync(Guid id) => _registrationRepository.GetByIdAsync(id);

    public Task<bool> ExistsAsync(Guid studentId, Guid classId) => _registrationRepository.ExistsAsync(studentId, classId);

    public List<Registration> GetRegistrationsByStudentId(Guid studentId) => _registrationRepository.GetRegistrationsByStudentId(studentId);

    public Task<List<Registration>> GetRegistrationsByStudentIdAsync(Guid studentId) => _registrationRepository.GetRegistrationsByStudentIdAsync(studentId);

    public List<Registration> GetRegistrationsByClassId(Guid classId) => _registrationRepository.GetRegistrationsByClassId(classId);

    public Task<List<Registration>> GetRegistrationsByClassIdAsync(Guid classId) => _registrationRepository.GetRegistrationsByClassIdAsync(classId);

    public List<Registration> GetAll() => _registrationRepository.GetAll();

    public Task<List<Registration>> GetAllAsync() => _registrationRepository.GetAllAsync();

    public void Add(Registration registration) => _registrationRepository.Add(registration);

    public Task AddAsync(Registration registration) => _registrationRepository.AddAsync(registration);

    public void Update(Guid id, Registration registration) => _registrationRepository.Update(id, registration);

    public Task UpdateAsync(Guid id, Registration registration) => _registrationRepository.UpdateAsync(id, registration);

    public void Delete(Guid id) => _registrationRepository.Delete(id);

    public Task DeleteAsync(Guid id) => _registrationRepository.DeleteAsync(id);

    public List<Registration> Search(string regex) => _registrationRepository.Search(regex);

    public Task<List<Registration>> SearchAsync(string regex) => _registrationRepository.SearchAsync(regex);

    public Task<List<(Registration Registration, Class Class)>> GetStudentRegistrationsWithClassesAsync(Guid studentId) =>
        _registrationRepository.GetStudentRegistrationsWithClassesAsync(studentId);
}
