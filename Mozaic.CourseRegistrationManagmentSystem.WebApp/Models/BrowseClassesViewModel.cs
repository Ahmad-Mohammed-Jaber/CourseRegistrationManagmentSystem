using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

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
        IMapper mapper,
        string? course,
        string? className,
        string? instructor,
        int pageSize)
    {
        return mapper.Map<BrowseClassesViewModel>(
            new BrowseClassesMappingSource(result, course, className, instructor, pageSize));
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
