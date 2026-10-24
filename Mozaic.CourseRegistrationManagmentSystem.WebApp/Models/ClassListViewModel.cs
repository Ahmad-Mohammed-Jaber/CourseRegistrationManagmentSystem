using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public sealed class ClassListViewModel
{
    public List<ClassListItemViewModel> Classes { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string ClassName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public string Course { get; set; } = string.Empty;

    public string Status { get; set; } = ClassResult.StatusActive;

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static ClassListViewModel FromResult(
        ClassResult result,
        string? className,
        string? instructor,
        string? course,
        string? status,
        int pageSize)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? ClassResult.StatusActive : status!;

        var routeValues = ListRouteValues.Create(
            pageSize,
            ("className", className),
            ("instructor", instructor),
            ("course", course),
            ("status", normalizedStatus));
        routeValues["status"] = normalizedStatus;

        return new ClassListViewModel
        {
            Classes = result.Select(c => new ClassListItemViewModel
            {
                Id = c.Id,
                CourseName = $"{c.Course?.CourseCode} {c.Course?.CourseName}".Trim(),
                ClassName = c.ClassName,
                Instructor = c.Instructor,
                Schedule = ScheduleHelper.GetScheduleString(c.Schedule),
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Seats = $"{c.CurrentCapacity}/{c.MaxCapacity}",
                IsActive = c.IsActive,
                CreatedOn = c.CreatedOn,
                CreatedByName = ResolveUserName(c.CreatedByName, c.CreatedBy),
                ModifiedOn = c.ModifiedOn,
                ModifiedByName = ResolveUserName(c.ModifiedByName, c.ModifiedBy),
            }).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            ClassName = className?.Trim() ?? string.Empty,
            Instructor = instructor?.Trim() ?? string.Empty,
            Course = course?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = routeValues,
        };
    }

    public static ClassListViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? className = null,
        string? instructor = null,
        string? course = null,
        string? status = null)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? ClassResult.StatusActive : status!;

        var routeValues = ListRouteValues.Create(
            pageSize,
            ("className", className),
            ("instructor", instructor),
            ("course", course),
            ("status", normalizedStatus));
        routeValues["status"] = normalizedStatus;

        return new ClassListViewModel
        {
            Page = page,
            PageSize = pageSize,
            ClassName = className?.Trim() ?? string.Empty,
            Instructor = instructor?.Trim() ?? string.Empty,
            Course = course?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = routeValues,
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
