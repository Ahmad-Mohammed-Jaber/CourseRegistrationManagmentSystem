using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

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
        IMapper mapper,
        string? className,
        string? instructor,
        string? course,
        string? status,
        int pageSize)
    {
        return mapper.Map<ClassListViewModel>(
            new ClassListMappingSource(result, className, instructor, course, status, pageSize));
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
}
