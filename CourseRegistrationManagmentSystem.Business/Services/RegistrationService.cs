using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Data.Repository;
using System.Text.RegularExpressions;

namespace CourseRegistrationManagmentSystem.Business.Services;
public class RegistrationService : ICrudService<RegistrationDto>
{
    private readonly RegistrationRepository _registrationRepository = new RegistrationRepository();

    public RegistrationDto? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        var registration = _registrationRepository.GetById(id);
        return registration.ToDto();
    }

    public async Task<RegistrationDto?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        var registration = await _registrationRepository.GetByIdAsync(id);
        return registration.ToDto();
    }

    public List<RegistrationDto> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _registrationRepository.GetAll().Select(registration => registration.ToDto()!).ToList();
    }

    public async Task<List<RegistrationDto>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var registrations = await _registrationRepository.GetAllAsync();
        return registrations.Select(registration => registration.ToDto()!).ToList();
    }

    public void Add(RegistrationDto registrationDto)
    {
        AccessValidator.RequireAdmin();
        var registration = registrationDto.ToEntity();
        _registrationRepository.Add(registration);
    }

    public async Task AddAsync(RegistrationDto registrationDto)
    {
        AccessValidator.RequireAdmin();
        var registration = registrationDto.ToEntity();
        await _registrationRepository.AddAsync(registration);
    }

    public void Update(Guid id, RegistrationDto registrationDto)
    {
        AccessValidator.RequireAdmin();
        var registration = _registrationRepository.GetById(id);
        if (registration == null) throw new KeyNotFoundException($"Registration with id {id} not found.");

        registration.StudentId = registrationDto.StudentId;
        registration.ClassId = registrationDto.ClassId;
        registration.RegsitrationDate = registrationDto.RegistrationDate;
        registration.Status = registrationDto.Status;

        _registrationRepository.Update(id, registration);
    }

    public async Task UpdateAsync(Guid id, RegistrationDto registrationDto)
    {
        AccessValidator.RequireAdmin();
        var registration = await _registrationRepository.GetByIdAsync(id);
        if (registration == null) throw new KeyNotFoundException($"Registration with id {id} not found.");

        registration.StudentId = registrationDto.StudentId;
        registration.ClassId = registrationDto.ClassId;
        registration.RegsitrationDate = registrationDto.RegistrationDate;
        registration.Status = registrationDto.Status;

        await _registrationRepository.UpdateAsync(id, registration);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _registrationRepository.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _registrationRepository.DeleteAsync(id);
    }

    public List<RegistrationDto> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var registrations = _registrationRepository.GetAll();
        return registrations
            .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
            .Select(registration => registration.ToDto()!)
            .ToList();
    }

    public async Task<List<RegistrationDto>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var registrations = await _registrationRepository.GetAllAsync();
        return registrations
            .Where(registration => Regex.IsMatch(registration.Status, regex, RegexOptions.IgnoreCase))
            .Select(registration => registration.ToDto()!)
            .ToList();
    }
}
