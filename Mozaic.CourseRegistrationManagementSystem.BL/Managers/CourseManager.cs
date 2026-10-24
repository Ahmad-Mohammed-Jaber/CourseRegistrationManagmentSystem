using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class CourseManager
{

    private readonly CourseRepository _courseRepository = new CourseRepository();
    public Task<CourseResult> GetAsync(
        int? id = null,
        string? courseCode = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null) => _courseRepository.GetAsync(id, courseCode, search, pageNumber, pageSize);

    public Task<Course?> GetByIdAsync(int id) => _courseRepository.GetByIdAsync(id);

    public void Add(Course course) => _courseRepository.Add(course);

    public Task AddAsync(Course course) => _courseRepository.AddAsync(course);

    public void Update(Course course) => _courseRepository.Update(course);

    public Task UpdateAsync(Course course) => _courseRepository.UpdateAsync(course);

    public void Delete(int id) => _courseRepository.Delete(id);

    public Task DeleteAsync(int id) => _courseRepository.DeleteAsync(id);

}
