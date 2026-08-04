using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Data.Database;
using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using Microsoft.Data.SqlClient;
using System.Data;
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

    public async Task<List<RegistrationDto>> GetAllDetailedAsync()
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = @"
        SELECT
            r.Id,
            r.StudentId,
            s.FullName,
            r.ClassId,
            c.ClassName,
            co.CourseName,
            r.RegistrationDate,
            r.Status
        FROM Registrations r
        INNER JOIN Student s 
            ON r.StudentId = s.Id
        INNER JOIN Class c 
            ON r.ClassId = c.Id
        INNER JOIN Course co 
            ON c.CourseId = co.Id";

        using var command = new SqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var registrations = new List<RegistrationDto>();

        while (await reader.ReadAsync())
        {
            registrations.Add(new RegistrationDto
            {
                Id = reader.GetGuid(0),

                StudentId = reader.GetGuid(1),
                StudentUserName = reader.GetString(2),


                ClassId = reader.GetGuid(3),
                ClassName = reader.GetString(4),

                CourseName = reader.GetString(5),

                RegistrationDate = reader.GetDateTime(6),
                Status = reader.GetString(7)
            });
        }

        return registrations;
    }

    public async Task<List<RegistrationDto>> SearchDetailedAsync(string regex)
    {
        using var connection = DBConnectionFactory.CreateConnection();
        await connection.OpenAsync();

        string sql = @"
        SELECT
            r.Id,
            r.StudentId,
            s.FullName,
            r.ClassId,
            c.ClassName,
            co.CourseName,
            r.RegistrationDate,
            r.Status
        FROM Registrations r
        INNER JOIN Student s 
            ON r.StudentId = s.Id
        INNER JOIN Class c 
            ON r.ClassId = c.Id
        INNER JOIN Course co 
            ON c.CourseId = co.Id
        WHERE 
            s.FullName LIKE @Search
            OR c.ClassName LIKE @Search
            OR co.CourseName LIKE @Search
            OR r.Status LIKE @Search";

        using var command = new SqlCommand(sql, connection);

        command.Parameters.Add("@Search", SqlDbType.NVarChar)
            .Value = $"%{regex}%";

        using var reader = await command.ExecuteReaderAsync();

        var registrations = new List<RegistrationDto>();

        while (await reader.ReadAsync())
        {
            registrations.Add(new RegistrationDto
            {
                Id = reader.GetGuid(0),

                StudentId = reader.GetGuid(1),
                StudentUserName = reader.GetString(2),

                ClassId = reader.GetGuid(3),
                ClassName = reader.GetString(4),

                CourseName = reader.GetString(5),

                RegistrationDate = reader.GetDateTime(6),
                Status = reader.GetString(7)
            });
        }

        return registrations;
    }
}
