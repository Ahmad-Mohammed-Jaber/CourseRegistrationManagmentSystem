using DAL.Interfaces;
using DAL.Providers;
using Shared.Entities;

namespace DAL.Repository;

public class RegistrationRepository : IGenericRepository<Registration>
{
    public Registration? GetById(int id)
    {
        return RegistrationDataProvider.GetById(id);
    }

    public Task<Registration?> GetByIdAsync(int id)
    {
        return RegistrationDataProvider.GetByIdAsync(id);
    }

    public Task<bool> ExistsAsync(int studentId, int classId)
    {
        return RegistrationDataProvider.ExistsAsync(studentId, classId);
    }

    public List<Registration> GetRegistrationsByStudentId(int studentId)
    {
        return RegistrationDataProvider.GetRegistrationsByStudentId(studentId);
    }

    public Task<List<Registration>> GetRegistrationsByStudentIdAsync(int studentId)
    {
        return RegistrationDataProvider.GetRegistrationsByStudentIdAsync(studentId);
    }

    public List<Registration> GetRegistrationsByClassId(int classId)
    {
        return RegistrationDataProvider.GetRegistrationsByClassId(classId);
    }

    public Task<List<Registration>> GetRegistrationsByClassIdAsync(int classId)
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

    public RequestResult Add(Registration entity)
    {
        return RegistrationDataProvider.Add(entity);
    }

    public Task<RequestResult> AddAsync(Registration entity)
    {
        return RegistrationDataProvider.AddAsync(entity);
    }

    public RequestResult Update(int id, Registration entity)
    {
        return RegistrationDataProvider.Update(id, entity);
    }

    public Task<RequestResult> UpdateAsync(int id, Registration entity)
    {
        return RegistrationDataProvider.UpdateAsync(id, entity);
    }

    public RequestResult Delete(int id)
    {
        return RegistrationDataProvider.Delete(id);
    }

    public Task<RequestResult> DeleteAsync(int id)
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

    public Task<List<(Registration Registration, Class Class)>> GetStudentRegistrationsWithClassesAsync(int studentId)
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