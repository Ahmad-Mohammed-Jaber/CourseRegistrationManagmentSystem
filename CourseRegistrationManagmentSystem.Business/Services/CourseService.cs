using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Managers;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Models;
using System.Text.RegularExpressions;

namespace CourseRegistrationManagmentSystem.Business.Services;

public class CourseService : ICrudService<Course>
{
    private readonly CourseManager _courseManager = new CourseManager();

    public Course? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        return _courseManager.GetById(id);
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        return await _courseManager.GetByIdAsync(id);
    }

    public List<Course> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _courseManager.GetAll();
    }

    public async Task<List<Course>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        return await _courseManager.GetAllAsync();
    }

    public void Add(Course course)
    {
        AccessValidator.RequireAdmin();
        // Business Logic / Validation here
        if (string.IsNullOrWhiteSpace(course.CourseCode)) throw new ArgumentException("Course code is required.");
        if (string.IsNullOrWhiteSpace(course.CourseName)) throw new ArgumentException("Course name is required.");

        _courseManager.Add(course);
    }

    public async Task AddAsync(Course course)
    {
        AccessValidator.RequireAdmin();
        // Business Logic / Validation here
        if (string.IsNullOrWhiteSpace(course.CourseCode)) throw new ArgumentException("Course code is required.");
        if (string.IsNullOrWhiteSpace(course.CourseName)) throw new ArgumentException("Course name is required.");

        await _courseManager.AddAsync(course);
    }

    public void Update(Guid id, Course course)
    {
        AccessValidator.RequireAdmin();
        var existingCourse = _courseManager.GetById(id);
        if (existingCourse == null) throw new KeyNotFoundException($"Course with id {id} not found.");

        // Validation
        if (string.IsNullOrWhiteSpace(course.CourseCode)) throw new ArgumentException("Course code is required.");
        if (string.IsNullOrWhiteSpace(course.CourseName)) throw new ArgumentException("Course name is required.");

        _courseManager.Update(id, course);
    }

    public async Task UpdateAsync(Guid id, Course course)
    {
        AccessValidator.RequireAdmin();
        var existingCourse = await _courseManager.GetByIdAsync(id);
        if (existingCourse == null) throw new KeyNotFoundException($"Course with id {id} not found.");

        // Validation
        if (string.IsNullOrWhiteSpace(course.CourseCode)) throw new ArgumentException("Course code is required.");
        if (string.IsNullOrWhiteSpace(course.CourseName)) throw new ArgumentException("Course name is required.");

        await _courseManager.UpdateAsync(id, course);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _courseManager.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _courseManager.DeleteAsync(id);
    }

    public List<Course> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        return _courseManager.Search(regex);
    }

    public async Task<List<Course>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        return await _courseManager.SearchAsync(regex);
    }
}
