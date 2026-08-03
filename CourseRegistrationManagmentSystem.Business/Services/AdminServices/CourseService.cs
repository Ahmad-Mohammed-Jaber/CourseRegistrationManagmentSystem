using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Data.Repository;
using System.Text.RegularExpressions;

public class CourseService : ICrudService<CourseDto>
{
    private readonly CourseRepository _courseRepository = new CourseRepository();

    public CourseDto? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        var course = _courseRepository.GetById(id);
        return course.ToDto();
    }

    public async Task<CourseDto?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        var course = await _courseRepository.GetByIdAsync(id);
        return course.ToDto();
    }

    public List<CourseDto> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _courseRepository.GetAll().Select(course => course.ToDto()!).ToList();
    }

    public async Task<List<CourseDto>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var courses = await _courseRepository.GetAllAsync();
        return courses.Select(course => course.ToDto()!).ToList();
    }

    public void Add(CourseDto courseDto)
    {
        AccessValidator.RequireAdmin();
        var course = courseDto.ToEntity();
        _courseRepository.Add(course);
    }

    public async Task AddAsync(CourseDto courseDto)
    {
        AccessValidator.RequireAdmin();
        var course = courseDto.ToEntity();
        await _courseRepository.AddAsync(course);
    }

    public void Update(Guid id, CourseDto courseDto)
    {
        AccessValidator.RequireAdmin();
        var course = _courseRepository.GetById(id);
        if (course == null) throw new KeyNotFoundException($"Course with id {id} not found.");

        course.CourseCode = courseDto.CourseCode;
        course.CourseName = courseDto.CourseName;
        course.CreditHours = courseDto.CreditHours;
        course.Description = courseDto.Description;
        course.IsActive = courseDto.IsActive;

        _courseRepository.Update(id, course);
    }

    public async Task UpdateAsync(Guid id, CourseDto courseDto)
    {
        AccessValidator.RequireAdmin();
        var course = await _courseRepository.GetByIdAsync(id);
        if (course == null) throw new KeyNotFoundException($"Course with id {id} not found.");

        course.CourseCode = courseDto.CourseCode;
        course.CourseName = courseDto.CourseName;
        course.CreditHours = courseDto.CreditHours;
        course.Description = courseDto.Description;
        course.IsActive = courseDto.IsActive;

        await _courseRepository.UpdateAsync(id, course);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _courseRepository.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _courseRepository.DeleteAsync(id);
    }

    public List<CourseDto> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var courses = _courseRepository.GetAll();
        return courses
            .Where(course => Regex.IsMatch(course.CourseCode, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(course.CourseName, regex, RegexOptions.IgnoreCase))
            .Select(course => course.ToDto()!)
            .ToList();
    }

    public async Task<List<CourseDto>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var courses = await _courseRepository.GetAllAsync();
        return courses
            .Where(course => Regex.IsMatch(course.CourseCode, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(course.CourseName, regex, RegexOptions.IgnoreCase))
            .Select(course => course.ToDto()!)
            .ToList();
    }
}
