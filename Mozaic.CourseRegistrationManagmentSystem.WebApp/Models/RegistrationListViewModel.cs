using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public sealed class RegistrationListViewModel
{
    public List<RegistrationListItemViewModel> Registrations { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string Student { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static RegistrationListViewModel FromResult(
        RegistrationResult result,
        string? student,
        string? course,
        string? className,
        string? status,
        int pageSize)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new RegistrationListViewModel
        {
            Registrations = result.Select(r => new RegistrationListItemViewModel
            {
                Id = r.Id,
                StudentName = $"{r.Student.FullName} ({r.Student.UserName})",
                CourseName = r.Course.CourseName,
                ClassName = r.Class.ClassName,
                RegistrationDate = r.RegistrationDate,
                Status = r.Status,
                CreatedOn = r.CreatedOn,
                CreatedByName = ResolveUserName(r.CreatedByName, r.CreatedBy),
                ModifiedOn = r.ModifiedOn,
                ModifiedByName = ResolveUserName(r.ModifiedByName, r.ModifiedBy),
            }).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            Student = student?.Trim() ?? string.Empty,
            Course = course?.Trim() ?? string.Empty,
            ClassName = className?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("student", student),
                ("course", course),
                ("className", className),
                ("status", normalizedStatus)),
        };
    }

    public static RegistrationListViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? student = null,
        string? course = null,
        string? className = null,
        string? status = null)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new RegistrationListViewModel
        {
            Page = page,
            PageSize = pageSize,
            Student = student?.Trim() ?? string.Empty,
            Course = course?.Trim() ?? string.Empty,
            ClassName = className?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("student", student),
                ("course", course),
                ("className", className),
                ("status", normalizedStatus)),
        };
    }

    private static string ResolveUserName(string? name, int userId)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        return userId <= 0 ? "—" : $"#{userId}";
    }
}
