using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class ClassManager
{

    private readonly ClassRepository _classRepository = new ClassRepository();
    public Task<ClassResult> GetAsync(
        int? id = null,
        int? courseId = null,
        int? userId = null,
        string? search = null,
        string? statusFilter = null,
        string? classNameFilter = null,
        string? instructorFilter = null,
        string? courseFilter = null,
        int? pageNumber = null,
        int? pageSize = null) => _classRepository.GetAsync(
            id, courseId, userId, search, statusFilter, classNameFilter, instructorFilter,
            courseFilter, pageNumber, pageSize);

    public Task<Class?> GetByIdAsync(int id) => _classRepository.GetByIdAsync(id);

    public void Add(Class classEntity) => _classRepository.Add(classEntity);

    public Task AddAsync(Class classEntity) => _classRepository.AddAsync(classEntity);

    public void Update(Class classEntity) => _classRepository.Update(classEntity);

    public Task UpdateAsync(Class classEntity) => _classRepository.UpdateAsync(classEntity);

    public void Delete(int id) => _classRepository.Delete(id);

    public Task DeleteAsync(int id) => _classRepository.DeleteAsync(id);

}
