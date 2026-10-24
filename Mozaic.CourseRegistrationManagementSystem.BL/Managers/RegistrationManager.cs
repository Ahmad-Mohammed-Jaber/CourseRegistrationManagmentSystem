using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class RegistrationManager
{

    private readonly RegistrationRepository _registrationRepository = new RegistrationRepository();
    public Task<RegistrationResult> GetAsync(
        int? id = null,
        int? studentId = null,
        int? classId = null,
        string? search = null,
        string? studentFilter = null,
        string? courseFilter = null,
        string? classFilter = null,
        string? statusFilter = null,
        int? pageNumber = null,
        int? pageSize = null) => _registrationRepository.GetAsync(
            id, studentId, classId, search, studentFilter, courseFilter, classFilter,
            statusFilter, pageNumber, pageSize);

    public Task<Registration?> GetByIdAsync(int id) => _registrationRepository.GetByIdAsync(id);

    public void Add(Registration registration) => _registrationRepository.Add(registration);

    public Task AddAsync(Registration registration) => _registrationRepository.AddAsync(registration);

    public void Update(Registration registration) => _registrationRepository.Update(registration);

    public Task UpdateAsync(Registration registration) => _registrationRepository.UpdateAsync(registration);

    public void Delete(int id) => _registrationRepository.Delete(id);

    public Task DeleteAsync(int id) => _registrationRepository.DeleteAsync(id);

}
