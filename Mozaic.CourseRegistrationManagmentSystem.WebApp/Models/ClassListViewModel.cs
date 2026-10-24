using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Strongly-typed page model for Views/Classes/Index.cshtml.
/// Holds the current page of classes plus the pager state, the search term
/// and the active status filter.
/// </summary>
public sealed class ClassListViewModel
{
    public List<ClassListItemViewModel> Classes { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    /// <summary>The active search term, echoed back into the search box.</summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>One of 'Active', 'Inactive' or 'All'.</summary>
    public string Status { get; set; } = ClassResult.StatusActive;

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static ClassListViewModel FromResult(ClassResult result, string? query, string? status)
    {
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
            Query = query?.Trim() ?? string.Empty,
            Status = string.IsNullOrWhiteSpace(status) ? ClassResult.StatusActive : status!,
        };
    }

    public static ClassListViewModel Empty(int page = 1, int pageSize = 20, string? query = null, string? status = null)
    {
        return new ClassListViewModel
        {
            Page = page,
            PageSize = pageSize,
            Query = query?.Trim() ?? string.Empty,
            Status = string.IsNullOrWhiteSpace(status) ? ClassResult.StatusActive : status!,
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
