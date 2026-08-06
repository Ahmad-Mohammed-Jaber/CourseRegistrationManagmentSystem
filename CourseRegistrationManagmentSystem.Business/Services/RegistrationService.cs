using BL.Interfaces;
using BL.Managers;
using BL.Validation;
using Shared.Entities;
using System.Text.RegularExpressions;

namespace BL.Services;

public class RegistrationService : ICrudService<Registration>
{
    private readonly RegistrationManager _registrationManager = new RegistrationManager();

    public Registration? GetById(int id)
    {
        AccessValidator.RequireAdmin();
        return _registrationManager.GetById(id);
    }

    public async Task<Registration?> GetByIdAsync(int id)
    {
        AccessValidator.RequireAdmin();
        return await _registrationManager.GetByIdAsync(id);
    }

    public List<Registration> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _registrationManager.GetAll();
    }

    public async Task<List<Registration>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        return await _registrationManager.GetAllAsync();
    }

    public void Add(Registration registration)
    {
        AccessValidator.RequireAdmin();
        ValidateRegistration(registration);
        _registrationManager.Add(registration);
    }

    public async Task AddAsync(Registration registration)
    {
        AccessValidator.RequireAdmin();
        ValidateRegistration(registration);
        await _registrationManager.AddAsync(registration);
    }

    public void Update(int id, Registration registration)
    {
        AccessValidator.RequireAdmin();
        ValidateRegistration(registration);

        var existing = _registrationManager.GetById(id);
        if (existing == null) throw new KeyNotFoundException($"Registration with id {id} not found.");

        _registrationManager.Update(id, registration);
    }

    public async Task UpdateAsync(int id, Registration registration)
    {
        AccessValidator.RequireAdmin();
        ValidateRegistration(registration);

        var existing = await _registrationManager.GetByIdAsync(id);
        if (existing == null) throw new KeyNotFoundException($"Registration with id {id} not found.");

        await _registrationManager.UpdateAsync(id, registration);
    }

    public void Delete(int id)
    {
        AccessValidator.RequireAdmin();
        _registrationManager.Delete(id);
    }

    public async Task DeleteAsync(int id)
    {
        AccessValidator.RequireAdmin();
        await _registrationManager.DeleteAsync(id);
    }

    public List<Registration> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var registrations = _registrationManager.GetAll();
        return registrations
            .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
            .ToList();
    }

    public async Task<List<Registration>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var registrations = await _registrationManager.GetAllAsync();
        return registrations
            .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
            .ToList();
    }

    public async Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> GetAllDetailedAsync()
    {
        AccessValidator.RequireAdmin();
        return await _registrationManager.GetAllDetailedAsync();
    }

    public async Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> SearchDetailedAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        return await _registrationManager.SearchDetailedAsync(regex);
    }

    private static void ValidateRegistration(Registration registration)
    {
        if (registration == null) throw new ArgumentNullException(nameof(registration));
        if (registration.StudentId <= 0) throw new ArgumentException("Student is required.");
        if (registration.ClassId <= 0) throw new ArgumentException("Class is required.");
        if (string.IsNullOrWhiteSpace(registration.Status)) throw new ArgumentException("Status is required.");
    }
}