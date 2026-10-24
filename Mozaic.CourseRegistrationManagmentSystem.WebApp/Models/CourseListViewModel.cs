using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Strongly-typed page model for Views/Courses/Index.cshtml.
/// Holds the current page of courses plus the pager state and column filters.
/// </summary>
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

    /// <summary>One of 'Active', 'Inactive' or 'All'.</summary>
    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static CourseListViewModel FromResult(
        CourseResult result,
        string? code,
        string? name,
        string? description,
        string? status,
        int pageSize)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new CourseListViewModel
        {
            Courses = result.Select(c => new CourseListItemViewModel
            {
                Id = c.Id,
                CourseCode = c.CourseCode,
                CourseName = c.CourseName,
                CreditHours = c.CreditHours,
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

    /// <summary>
    /// Falls back to the raw id when the audit actor row is missing, matching
    /// the placeholder the controller used to render.
    /// </summary>
    private static string ResolveUserName(string? name, int userId)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        return userId <= 0 ? "—" : $"#{userId}";
    }
}
