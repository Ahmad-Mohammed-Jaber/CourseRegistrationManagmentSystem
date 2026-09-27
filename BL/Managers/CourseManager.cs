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

    public int Add(Course course) => _courseRepository.Add(course);

    public Task<int > AddAsync(Course course) => _courseRepository.AddAsync(course);

    public int Update(int id, Course course) => _courseRepository.Update(id, course);

    public Task<int > UpdateAsync(int id, Course course) => _courseRepository.UpdateAsync(id, course);

    public int Delete(int id) => _courseRepository.Delete(id);

    public Task<int > DeleteAsync(int id) => _courseRepository.DeleteAsync(id);

    public List<Course> Search(string regex) => _courseRepository.Search(regex);

    public Task<List<Course>> SearchAsync(string regex) => _courseRepository.SearchAsync(regex);
}
