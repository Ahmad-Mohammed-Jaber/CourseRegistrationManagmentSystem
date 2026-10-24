using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class StudentManager
{

    private readonly StudentRepository _studentRepository = new StudentRepository();
    public Task<StudentResult> GetAsync(
        int? id = null,
        int? userId = null,
        string? search = null,
        string? studentNumberFilter = null,
        string? fullNameFilter = null,
        string? userNameFilter = null,
        string? emailFilter = null,
        string? phoneFilter = null,
        bool? isActive = null,
        int? pageNumber = null,
        int? pageSize = null) => _studentRepository.GetAsync(
            id, userId, search, studentNumberFilter, fullNameFilter, userNameFilter,
            emailFilter, phoneFilter, isActive, pageNumber, pageSize);

    public Task<Student?> GetByIdAsync(int id) => _studentRepository.GetByIdAsync(id);

    public void Add(Student student) => _studentRepository.Add(student);

    public Task AddAsync(Student student) => _studentRepository.AddAsync(student);

    public void Update(Student student) => _studentRepository.Update(student);

    public Task UpdateAsync(Student student) => _studentRepository.UpdateAsync(student);

    public void Delete(int id) => _studentRepository.Delete(id);

    public Task DeleteAsync(int id) => _studentRepository.DeleteAsync(id);

}
