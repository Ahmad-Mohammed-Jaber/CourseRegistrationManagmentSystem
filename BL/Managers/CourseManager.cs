using DAL.Repository;
using Shared.Entities;

namespace BL.Managers;

internal class CourseManager
{
    private readonly CourseRepository _courseRepository = new CourseRepository();

    public Course? GetById(int id) => _courseRepository.GetById(id);

    public Task<Course?> GetByIdAsync(int id) => _courseRepository.GetByIdAsync(id);

    public List<Course> GetAll() => _courseRepository.GetAll();

    public Task<List<Course>> GetAllAsync() => _courseRepository.GetAllAsync();

    public RequestResult Add(Course course) => _courseRepository.Add(course);

    public Task<RequestResult> AddAsync(Course course) => _courseRepository.AddAsync(course);

    public RequestResult Update(int id, Course course) => _courseRepository.Update(id, course);

    public Task<RequestResult> UpdateAsync(int id, Course course) => _courseRepository.UpdateAsync(id, course);

    public RequestResult Delete(int id) => _courseRepository.Delete(id);

    public Task<RequestResult> DeleteAsync(int id) => _courseRepository.DeleteAsync(id);

    public List<Course> Search(string regex) => _courseRepository.Search(regex);

    public Task<List<Course>> SearchAsync(string regex) => _courseRepository.SearchAsync(regex);
}