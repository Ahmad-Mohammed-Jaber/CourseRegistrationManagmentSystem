using CourseRegistrationManagmentSystem.Business.Interfaces;
using CourseRegistrationManagmentSystem.Business.Validation;
using CourseRegistrationManagmentSystem.Shared.Dtos;
using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Repository;
using System.Linq;
using System.Text.RegularExpressions;

public class StudentService : IGenericService<StudentDto>
{
    private readonly StudentRepository _studentRepository = new StudentRepository();

    public StudentDto? GetById(Guid id)
    {
        AccessValidator.RequireAdmin();
        var student = _studentRepository.GetById(id);
        return student.ToDto();
    }

    public async Task<StudentDto?> GetByIdAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        var student = await _studentRepository.GetByIdAsync(id);
        return student.ToDto();
    }

    public List<StudentDto> GetAll()
    {
        AccessValidator.RequireAdmin();
        return _studentRepository.GetAll().Select(student => student.ToDto()!).ToList();
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        AccessValidator.RequireAdmin();
        var students = await _studentRepository.GetAllAsync();
        return students.Select(student => student.ToDto()!).ToList();
    }

    public void Add(StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();
        var student = studentDto.ToEntity();
        _studentRepository.Add(student);
    }

    public async Task AddAsync(StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();
        var student = studentDto.ToEntity();
        await _studentRepository.AddAsync(student);
    }

    public void Update(Guid id, StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();
        var student = _studentRepository.GetById(id);
        if (student == null) throw new KeyNotFoundException($"Student with id {id} not found.");

        student.UserId = studentDto.Id;
        student.StudentNumber = studentDto.StudentNumber;
        student.FullName = studentDto.FullName;
        student.Email = studentDto.Email;
        student.Phone = studentDto.Phone;

        _studentRepository.Update(id, student);
    }

    public async Task UpdateAsync(Guid id, StudentDto studentDto)
    {
        AccessValidator.RequireAdmin();
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null) throw new KeyNotFoundException($"Student with id {id} not found.");

        student.UserId = studentDto.Id;
        student.StudentNumber = studentDto.StudentNumber;
        student.FullName = studentDto.FullName;
        student.Email = studentDto.Email;
        student.Phone = studentDto.Phone;

        await _studentRepository.UpdateAsync(id, student);
    }

    public void Delete(Guid id)
    {
        AccessValidator.RequireAdmin();
        _studentRepository.Delete(id);
    }

    public async Task DeleteAsync(Guid id)
    {
        AccessValidator.RequireAdmin();
        await _studentRepository.DeleteAsync(id);
    }

    public List<StudentDto> Search(string regex)
    {
        AccessValidator.RequireAdmin();
        var students = _studentRepository.GetAll();
        return students
            .Where(student => Regex.IsMatch(student.FullName, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(student.Email, regex, RegexOptions.IgnoreCase))
            .Select(student => student.ToDto()!)
            .ToList();
    }

    public async Task<List<StudentDto>> SearchAsync(string regex)
    {
        AccessValidator.RequireAdmin();
        var students = await _studentRepository.GetAllAsync();
        return students
            .Where(student => Regex.IsMatch(student.FullName, regex, RegexOptions.IgnoreCase) ||
                        Regex.IsMatch(student.Email, regex, RegexOptions.IgnoreCase))
            .Select(student => student.ToDto()!)
            .ToList();
    }
}
