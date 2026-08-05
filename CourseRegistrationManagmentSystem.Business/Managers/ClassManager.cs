using CourseRegistrationManagmentSystem.Data.Repository;
using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Business.Managers;

internal class ClassManager
{
    private readonly ClassRepository _classRepository = new ClassRepository();

    public Class? GetById(Guid id) => _classRepository.GetById(id);

    public Task<Class?> GetByIdAsync(Guid id) => _classRepository.GetByIdAsync(id);

    public List<Class> GetClassesByCourseId(Guid courseId) => _classRepository.GetClassesByCourseId(courseId);

    public Task<List<Class>> GetClassesByCourseIdAsync(Guid courseId) => _classRepository.GetClassesByCourseIdAsync(courseId);

    public List<Class> GetAll() => _classRepository.GetAll();

    public Task<List<Class>> GetAllAsync() => _classRepository.GetAllAsync();

    public void Add(Class classEntity) => _classRepository.Add(classEntity);

    public Task AddAsync(Class classEntity) => _classRepository.AddAsync(classEntity);

    public void Update(Guid id, Class classEntity) => _classRepository.Update(id, classEntity);

    public Task UpdateAsync(Guid id, Class classEntity) => _classRepository.UpdateAsync(id, classEntity);

    public void Delete(Guid id) => _classRepository.Delete(id);

    public Task DeleteAsync(Guid id) => _classRepository.DeleteAsync(id);

    public List<Class> Search(string regex) => _classRepository.Search(regex);

    public Task<List<Class>> SearchAsync(string regex) => _classRepository.SearchAsync(regex);
}
