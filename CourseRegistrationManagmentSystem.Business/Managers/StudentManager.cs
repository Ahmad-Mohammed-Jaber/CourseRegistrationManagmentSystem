using DAL.Repository;
using Shared.Entities;

namespace BL.Managers;

internal class StudentManager
{
    private readonly StudentRepository _studentRepository = new StudentRepository();

    public Student? GetById(int id) => _studentRepository.GetById(id);

    public Task<Student?> GetByIdAsync(int id) => _studentRepository.GetByIdAsync(id);

    public Task<Student?> GetByUserIdAsync(int userId) => _studentRepository.GetByUserIdAsync(userId);

    public List<Student> GetAll() => _studentRepository.GetAll();

    public Task<List<Student>> GetAllAsync() => _studentRepository.GetAllAsync();

    public void Add(Student student) => _studentRepository.Add(student);

    public Task AddAsync(Student student) => _studentRepository.AddAsync(student);

    public void Update(int id, Student student) => _studentRepository.Update(id, student);

    public Task UpdateAsync(int id, Student student) => _studentRepository.UpdateAsync(id, student);

    public void Delete(int id) => _studentRepository.Delete(id);

    public Task DeleteAsync(int id) => _studentRepository.DeleteAsync(id);

    public List<Student> Search(string regex) => _studentRepository.Search(regex);

    public Task<List<Student>> SearchAsync(string regex) => _studentRepository.SearchAsync(regex);
}