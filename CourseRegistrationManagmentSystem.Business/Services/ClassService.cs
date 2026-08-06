using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.Entities;
using System.Text.RegularExpressions;

namespace BL.Services;

public class ClassService : ICrudService<Class>
{
    private readonly ClassManager _classManager = new ClassManager();

    public Class? GetById(int id)
    {
        AccessValidator.RequireLogin();
        return _classManager.GetById(id);
    }

    public async Task<Class?> GetByIdAsync(int id)
    {
        AccessValidator.RequireLogin();
        return await _classManager.GetByIdAsync(id);
    }

    public List<Class> GetAll()
    {
        AccessValidator.RequireLogin();
        return _classManager.GetAll();
    }

    public async Task<List<Class>> GetAllAsync()
    {
        AccessValidator.RequireLogin();
        return await _classManager.GetAllAsync();
    }

    public void Add(Class classEntity)
    {
        AccessValidator.RequireAdmin();
        // Validation
        if (string.IsNullOrWhiteSpace(classEntity.ClassName)) throw new ArgumentException("Class name is required.");
        if (classEntity.MaxCapacity <= 0) throw new ArgumentException("Max capacity must be greater than 0.");

        _classManager.Add(classEntity);
    }

    public async Task AddAsync(Class classEntity)
    {
        AccessValidator.RequireAdmin();
        // Validation
        if (string.IsNullOrWhiteSpace(classEntity.ClassName)) throw new ArgumentException("Class name is required.");
        if (classEntity.MaxCapacity <= 0) throw new ArgumentException("Max capacity must be greater than 0.");

        await _classManager.AddAsync(classEntity);
    }

    public void Update(int id, Class classEntity)
    {
        AccessValidator.RequireAdmin();
        var existingClass = _classManager.GetById(id);
        if (existingClass == null) throw new KeyNotFoundException($"Class with id {id} not found.");

        // Validation
        if (string.IsNullOrWhiteSpace(classEntity.ClassName)) throw new ArgumentException("Class name is required.");
        if (classEntity.MaxCapacity <= 0) throw new ArgumentException("Max capacity must be greater than 0.");

        _classManager.Update(id, classEntity);
    }

    public async Task UpdateAsync(int id, Class classEntity)
    {
        AccessValidator.RequireAdmin();
        var existingClass = await _classManager.GetByIdAsync(id);
        if (existingClass == null) throw new KeyNotFoundException($"Class with id {id} not found.");

        // Validation
        if (string.IsNullOrWhiteSpace(classEntity.ClassName)) throw new ArgumentException("Class name is required.");
        if (classEntity.MaxCapacity <= 0) throw new ArgumentException("Max capacity must be greater than 0.");

        await _classManager.UpdateAsync(id, classEntity);
    }

    public void Delete(int id)
    {
        AccessValidator.RequireAdmin();
        _classManager.Delete(id);
    }

    public async Task DeleteAsync(int id)
    {
        AccessValidator.RequireAdmin();
        await _classManager.DeleteAsync(id);
    }

    public List<Class> Search(string regex)
    {
        AccessValidator.RequireLogin();
        var classes = _classManager.GetAll();
        return classes
            .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                                  Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
            .ToList();
    }

    public async Task<List<Class>> SearchAsync(string regex)
    {
        AccessValidator.RequireLogin();
        var classes = await _classManager.GetAllAsync();
        return classes
            .Where(classEntity => Regex.IsMatch(classEntity.ClassName, regex, RegexOptions.IgnoreCase) ||
                                  Regex.IsMatch(classEntity.Instructor, regex, RegexOptions.IgnoreCase))
            .ToList();
    }
}