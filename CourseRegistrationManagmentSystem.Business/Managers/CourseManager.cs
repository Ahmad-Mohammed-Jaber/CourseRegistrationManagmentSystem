using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Managers;

internal class CourseManager
{
    private readonly CourseRepository _courseRepository = new CourseRepository();

    public Course? GetById(Guid id) => _courseRepository.GetById(id);

    public Task<Course?> GetByIdAsync(Guid id) => _courseRepository.GetByIdAsync(id);

    public List<Course> GetAll() => _courseRepository.GetAll();

    public Task<List<Course>> GetAllAsync() => _courseRepository.GetAllAsync();

    public void Add(Course course) => _courseRepository.Add(course);

    public Task AddAsync(Course course) => _courseRepository.AddAsync(course);

    public void Update(Guid id, Course course) => _courseRepository.Update(id, course);

    public Task UpdateAsync(Guid id, Course course) => _courseRepository.UpdateAsync(id, course);

    public void Delete(Guid id) => _courseRepository.Delete(id);

    public Task DeleteAsync(Guid id) => _courseRepository.DeleteAsync(id);

    public List<Course> Search(string regex) => _courseRepository.Search(regex);

    public Task<List<Course>> SearchAsync(string regex) => _courseRepository.SearchAsync(regex);
}
