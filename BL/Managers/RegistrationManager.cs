using DAL.Repository;
using Shared.Entities;

namespace BL.Managers;

internal class RegistrationManager
{
    private readonly RegistrationRepository _registrationRepository = new RegistrationRepository();

    public Registration? GetById(int id) => _registrationRepository.GetById(id);

    public Task<Registration?> GetByIdAsync(int id) => _registrationRepository.GetByIdAsync(id);

    public Task<bool> ExistsAsync(int studentId, int classId) => _registrationRepository.ExistsAsync(studentId, classId);

    public List<Registration> GetRegistrationsByStudentId(int studentId) => _registrationRepository.GetRegistrationsByStudentId(studentId);

    public Task<List<Registration>> GetRegistrationsByStudentIdAsync(int studentId) => _registrationRepository.GetRegistrationsByStudentIdAsync(studentId);

    public List<Registration> GetRegistrationsByClassId(int classId) => _registrationRepository.GetRegistrationsByClassId(classId);

    public Task<List<Registration>> GetRegistrationsByClassIdAsync(int classId) => _registrationRepository.GetRegistrationsByClassIdAsync(classId);

    public List<Registration> GetAll() => _registrationRepository.GetAll();

    public Task<List<Registration>> GetAllAsync() => _registrationRepository.GetAllAsync();

    public void Add(Registration registration) => _registrationRepository.Add(registration);

    public Task AddAsync(Registration registration) => _registrationRepository.AddAsync(registration);

    public void Update(int id, Registration registration) => _registrationRepository.Update(id, registration);

    public Task UpdateAsync(int id, Registration registration) => _registrationRepository.UpdateAsync(id, registration);

    public void Delete(int id) => _registrationRepository.Delete(id);

    public Task DeleteAsync(int id) => _registrationRepository.DeleteAsync(id);

    public List<Registration> Search(string regex) => _registrationRepository.Search(regex);

    public Task<List<Registration>> SearchAsync(string regex) => _registrationRepository.SearchAsync(regex);

    public Task<List<(Registration Registration, Class Class)>> GetStudentRegistrationsWithClassesAsync(int studentId) =>
        _registrationRepository.GetStudentRegistrationsWithClassesAsync(studentId);

    public Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> GetAllDetailedAsync() =>
        _registrationRepository.GetAllDetailedAsync();

    public Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> SearchDetailedAsync(string regex) =>
        _registrationRepository.SearchDetailedAsync(regex);
}