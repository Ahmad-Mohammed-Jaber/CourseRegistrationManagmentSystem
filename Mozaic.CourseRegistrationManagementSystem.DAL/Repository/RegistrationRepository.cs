using Mozaic.CourseRegistrationManagementSystem.DAL.Interfaces;
using Mozaic.CourseRegistrationManagementSystem.DAL.Providers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.DAL.Repository;

public class RegistrationRepository : IGenericRepository<Registration>
{
    public Task<RegistrationResult> GetAsync(
        int? id = null,
        int? studentId = null,
        int? classId = null,
        string? search = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        return RegistrationDataProvider.GetAsync(id, studentId, classId, search, pageNumber, pageSize);
    }
    public void Add(Registration entity)
    {
        RegistrationDataProvider.Add(entity);
    }

    public Task AddAsync(Registration entity)
    {
        return RegistrationDataProvider.AddAsync(entity);
    }

    public void Update(Registration entity)
    {
        RegistrationDataProvider.Update(entity);
    }

    public Task UpdateAsync(Registration entity)
    {
        return RegistrationDataProvider.UpdateAsync(entity);
    }

    public void Delete(int id)
    {
        RegistrationDataProvider.Delete(id);
    }

    public Task DeleteAsync(int id)
    {
        return RegistrationDataProvider.DeleteAsync(id);
    }

}
