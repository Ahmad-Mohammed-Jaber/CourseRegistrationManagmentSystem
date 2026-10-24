using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Strongly-typed page model for Views/Registrations/Mine.cshtml.
/// Holds the current page of the student's registrations plus pager state and filters.
/// </summary>
public sealed class MyRegistrationsListViewModel
{
    public List<MyRegistrationViewModel> Registrations { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string Course { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    /// <summary>One of 'Registered', 'Dropped' or 'All'.</summary>
    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static MyRegistrationsListViewModel FromResult(
        RegistrationResult result,
        string? course,
        string? className,
        string? status,
        int pageSize)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new MyRegistrationsListViewModel
        {
            Registrations = result.Select(r => new MyRegistrationViewModel
            {
                RegistrationId = r.Id,
                ClassId = r.Class.Id,
                CourseName = r.Course.CourseName,
                ClassName = r.Class.ClassName,
                Instructor = r.Class.Instructor,
                Schedule = ScheduleHelper.GetScheduleString(r.Class.Schedule),
                StartDate = r.Class.StartDate,
                EndDate = r.Class.EndDate,
                RegistrationDate = r.RegistrationDate,
                Status = r.Status,
            }).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            Course = course?.Trim() ?? string.Empty,
            ClassName = className?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("course", course),
                ("className", className),
                ("status", normalizedStatus)),
        };
    }

    public static MyRegistrationsListViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? course = null,
        string? className = null,
        string? status = null)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new MyRegistrationsListViewModel
        {
            Page = page,
            PageSize = pageSize,
            Course = course?.Trim() ?? string.Empty,
            ClassName = className?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("course", course),
                ("className", className),
                ("status", normalizedStatus)),
        };
    }
}
