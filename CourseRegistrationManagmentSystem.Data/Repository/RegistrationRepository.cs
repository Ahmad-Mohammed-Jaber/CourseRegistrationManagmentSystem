using CourseRegistrationManagmentSystem.Shared.Models;
using CourseRegistrationManagmentSystem.Data.Interfaces;
using CourseRegistrationManagmentSystem.Data.Providers;

namespace CourseRegistrationManagmentSystem.Data.Repository;

public class RegistrationRepository : IGenericRepository<Registration>
{
    public Registration? GetById(Guid id)
    {
        return RegistrationDataProvider.GetById(id);
    }

    public Task<Registration?> GetByIdAsync(Guid id)
    {
        return RegistrationDataProvider.GetByIdAsync(id);
    }

    public Task<bool> ExistsAsync(Guid studentId, Guid classId)
    {
        return RegistrationDataProvider.ExistsAsync(studentId, classId);
    }

    public List<Registration> GetRegistrationsByStudentId(Guid studentId)
    {
        return RegistrationDataProvider.GetRegistrationsByStudentId(studentId);
    }

    public Task<List<Registration>> GetRegistrationsByStudentIdAsync(Guid studentId)
    {
        return RegistrationDataProvider.GetRegistrationsByStudentIdAsync(studentId);
    }

    public List<Registration> GetRegistrationsByClassId(Guid classId)
    {
        return RegistrationDataProvider.GetRegistrationsByClassId(classId);
    }

    public Task<List<Registration>> GetRegistrationsByClassIdAsync(Guid classId)
    {
        return RegistrationDataProvider.GetRegistrationsByClassIdAsync(classId);
    }

    public List<Registration> GetAll()
    {
        return RegistrationDataProvider.GetAll();
    }

    public Task<List<Registration>> GetAllAsync()
    {
        return RegistrationDataProvider.GetAllAsync();
    }

    public void Add(Registration entity)
    {
        RegistrationDataProvider.Add(entity);
    }

    public Task AddAsync(Registration entity)
    {
        return RegistrationDataProvider.AddAsync(entity);
    }

    public void Update(Guid id, Registration entity)
    {
        RegistrationDataProvider.Update(id, entity);
    }

    public Task UpdateAsync(Guid id, Registration entity)
    {
        return RegistrationDataProvider.UpdateAsync(id, entity);
    }

    public void Delete(Guid id)
    {
        RegistrationDataProvider.Delete(id);
    }

    public Task DeleteAsync(Guid id)
    {
        return RegistrationDataProvider.DeleteAsync(id);
    }

    public List<Registration> Search(string regex)
    {
        return RegistrationDataProvider.Search(regex);
    }

    public Task<List<Registration>> SearchAsync(string regex)
    {
        return RegistrationDataProvider.SearchAsync(regex);
    }

    public Task<List<(Registration Registration, Class Class)>> GetStudentRegistrationsWithClassesAsync(Guid studentId)
    {
        return RegistrationDataProvider.GetStudentRegistrationsWithClassesAsync(studentId);
    }

    public Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> GetAllDetailedAsync()
    {
        return RegistrationDataProvider.GetAllDetailedAsync();
    }

    public Task<List<(Registration Registration, Student Student, Class Class, Course Course)>> SearchDetailedAsync(string regex)
    {
        return RegistrationDataProvider.SearchDetailedAsync(regex);
    }
}
