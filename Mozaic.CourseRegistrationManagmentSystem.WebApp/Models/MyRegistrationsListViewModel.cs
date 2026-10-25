using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public sealed class MyRegistrationsListViewModel
{
    public List<MyRegistrationViewModel> Registrations { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string Course { get; set; } = string.Empty;

    public string ClassName { get; set; } = string.Empty;

    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static MyRegistrationsListViewModel FromResult(
        RegistrationResult result,
        IMapper mapper,
        string? course,
        string? className,
        string? status,
        int pageSize)
    {
        return mapper.Map<MyRegistrationsListViewModel>(
            new MyRegistrationsListMappingSource(result, course, className, status, pageSize));
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
