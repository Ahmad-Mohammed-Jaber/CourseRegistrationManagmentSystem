using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Strongly-typed page model for Views/Classes/Browse.cshtml.
/// Replaces the ViewData bag (Page, PageSize, TotalPages, TotalCount,
/// AvailableCount, OpenSeatsCount, MyRegistrationsCount).
/// </summary>
public sealed class BrowseClassesViewModel
{
    public List<BrowseClassViewModel> Classes { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public int OpenSeatsCount { get; set; }

    public int MyRegistrationsCount { get; set; }

    public string Course { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static BrowseClassesViewModel FromResult(
        ClassResult result,
        string? course,
        string? className,
        string? instructor,
        int pageSize)
    {
        return new BrowseClassesViewModel
        {
            Classes = result.Select(@class => new BrowseClassViewModel
            {
                ClassId = @class.Id,
                CourseName = @class.Course?.CourseName ?? string.Empty,
                ClassName = @class.ClassName,
                Instructor = @class.Instructor,
                Schedule = ScheduleHelper.GetScheduleString(@class.Schedule),
                StartDate = @class.StartDate,
                EndDate = @class.EndDate,
                CurrentCapacity = @class.CurrentCapacity,
                MaxCapacity = @class.MaxCapacity,
                IsRegistered = @class.IsRegistered,
            }).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            OpenSeatsCount = result.OpenSeatsCount,
            MyRegistrationsCount = result.MyRegistrationsCount,
            Course = course?.Trim() ?? string.Empty,
            ClassName = className?.Trim() ?? string.Empty,
            Instructor = instructor?.Trim() ?? string.Empty,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("course", course),
                ("className", className),
                ("instructor", instructor)),
        };
    }

    public static BrowseClassesViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? course = null,
        string? className = null,
        string? instructor = null)
    {
        return new BrowseClassesViewModel
        {
            Page = page,
            PageSize = pageSize,
            Course = course?.Trim() ?? string.Empty,
            ClassName = className?.Trim() ?? string.Empty,
            Instructor = instructor?.Trim() ?? string.Empty,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("course", course),
                ("className", className),
                ("instructor", instructor)),
        };
    }
}
