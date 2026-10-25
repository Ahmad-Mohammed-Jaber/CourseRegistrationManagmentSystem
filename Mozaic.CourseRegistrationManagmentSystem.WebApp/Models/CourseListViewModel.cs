using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public sealed class CourseListViewModel
{
    public List<CourseListItemViewModel> Courses { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static CourseListViewModel FromResult(
        CourseResult result,
        IMapper mapper,
        string? code,
        string? name,
        string? description,
        string? status,
        int pageSize)
    {
        return mapper.Map<CourseListViewModel>(
            new CourseListMappingSource(result, code, name, description, status, pageSize));
    }

    public static CourseListViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? code = null,
        string? name = null,
        string? description = null,
        string? status = null)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new CourseListViewModel
        {
            Page = page,
            PageSize = pageSize,
            Code = code?.Trim() ?? string.Empty,
            Name = name?.Trim() ?? string.Empty,
            Description = description?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("code", code),
                ("name", name),
                ("description", description),
                ("status", normalizedStatus)),
        };
    }
}
