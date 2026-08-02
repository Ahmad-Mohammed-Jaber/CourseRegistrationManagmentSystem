using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Repository;
using System.Linq;
using System.Text.RegularExpressions;

public class ClassService : IGenericService<ClassDto>
{
    private readonly ClassRepository _classRepository = new ClassRepository();

    public ClassDto? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        var classEntity = _classRepository.GetById(id);
        return classEntity.ToDto();
    }

    public async Task<ClassDto?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        var classEntity = await _classRepository.GetByIdAsync(id);
        return classEntity.ToDto();
    }

    public List<ClassDto> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _classRepository.GetAll().Select(classEntity => classEntity.ToDto()!).ToList();
    }

    public async Task<List<ClassDto>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var classes = await _classRepository.GetAllAsync();
        return classes.Select(classEntity => classEntity.ToDto()!).ToList();
    }

    public void Add(ClassDto classDto)
    {
        AccessValidator.RequireAdmin();
        var classEntity = classDto.ToEntity();
        _classRepository.Add(classEntity);
    }

    public async Task AddAsync(ClassDto classDto)
    {
        AccessValidator.RequireAdmin();
        var classEntity = classDto.ToEntity();
        await _classRepository.AddAsync(classEntity);
    }

    public void Update(Guid id, ClassDto classDto)
    {
        AccessValidator.RequireAdmin();
        var classEntity = _classRepository.GetById(id);
        if (classEntity == null) throw new KeyNotFoundException($"Class with id {id} not found.");

        classEntity.CourseId = classDto.CourseId;
        classEntity.ClassName = classDto.ClassName;
        classEntity.Instructor = classDto.Instructor;
        classEntity.Capacity = classDto.Capacity;
        classEntity.StartDate = classDto.StartDate;
        classEntity.EndDate = classDto.EndDate;
        classEntity.Schedule = classDto.Schedule;
        classEntity.IsActive = classDto.IsActive;

        _classRepository.Update(id, classEntity);
    }

    public async Task UpdateAsync(Guid id, ClassDto classDto)
    {
        AccessValidator.RequireAdmin();
        var classEntity = await _classRepository.GetByIdAsync(id);
        if (classEntity == null) throw new KeyNotFoundException($"Class with id {id} not found.");

        classEntity.CourseId = classDto.CourseId;
        classEntity.ClassName = classDto.ClassName;
        classEntity.Instructor = classDto.Instructor;
        classEntity.Capacity = classDto.Capacity;
        classEntity.StartDate = classDto.StartDate;
        classEntity.EndDate = classDto.EndDate;
        classEntity.Schedule = classDto.Schedule;
        classEntity.IsActive = classDto.IsActive;

        await _classRepository.UpdateAsync(id, classEntity);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _classRepository.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _classRepository.DeleteAsync(id);
    }

    public List<ClassDto> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var classes = _classRepository.GetAll();
        return classes
            .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
            .Select(classEntity => classEntity.ToDto()!)
            .ToList();
    }

    public async Task<List<ClassDto>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var classes = await _classRepository.GetAllAsync();
        return classes
            .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
            .Select(classEntity => classEntity.ToDto()!)
            .ToList();
    }
}
