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

    public void Add(Course course) => _courseRepository.Add(course);

    public Task AddAsync(Course course) => _courseRepository.AddAsync(course);

    public void Update(int id, Course course) => _courseRepository.Update(id, course);

    public Task UpdateAsync(int id, Course course) => _courseRepository.UpdateAsync(id, course);

    public void Delete(int id) => _courseRepository.Delete(id);

    public Task DeleteAsync(int id) => _courseRepository.DeleteAsync(id);

    public List<Course> Search(string regex) => _courseRepository.Search(regex);

    public Task<List<Course>> SearchAsync(string regex) => _courseRepository.SearchAsync(regex);
}