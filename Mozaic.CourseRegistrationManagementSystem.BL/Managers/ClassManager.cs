using Mozaic.CourseRegistrationManagementSystem.DAL.Repository;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

namespace Mozaic.CourseRegistrationManagementSystem.BL.Managers;

internal class ClassManager
{
    private readonly ClassRepository _classRepository = new ClassRepository();

    public Class? GetById(int id) => _classRepository.GetById(id);

    public Task<Class?> GetByIdAsync(int id) => _classRepository.GetByIdAsync(id);

    public List<Class> GetClassesByCourseId(int courseId) => _classRepository.GetClassesByCourseId(courseId);

    public Task<List<Class>> GetClassesByCourseIdAsync(int courseId) => _classRepository.GetClassesByCourseIdAsync(courseId);

    public List<Class> GetAll() => _classRepository.GetAll();

    public Task<List<Class>> GetAllAsync() => _classRepository.GetAllAsync();

    public int Add(Class classEntity) => _classRepository.Add(classEntity);

    public Task<int > AddAsync(Class classEntity) => _classRepository.AddAsync(classEntity);

    public int Update(int id, Class classEntity) => _classRepository.Update(id, classEntity);

    public Task<int > UpdateAsync(int id, Class classEntity) => _classRepository.UpdateAsync(id, classEntity);

    public int Delete(int id) => _classRepository.Delete(id);

    public Task<int > DeleteAsync(int id) => _classRepository.DeleteAsync(id);

    public List<Class> Search(string regex) => _classRepository.Search(regex);

    public Task<List<Class>> SearchAsync(string regex) => _classRepository.SearchAsync(regex);
}
