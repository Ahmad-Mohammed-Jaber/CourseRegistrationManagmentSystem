using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Strongly-typed page model for Views/Students/Index.cshtml.
/// Holds the current page of students plus the pager state and column filters.
/// </summary>
public sealed class StudentListViewModel
{
    public List<StudentListItemViewModel> Students { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string StudentNumber { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    /// <summary>One of 'Active', 'Inactive' or 'All'.</summary>
    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static StudentListViewModel FromResult(
        StudentResult result,
        string? studentNumber,
        string? fullName,
        string? userName,
        string? email,
        string? phone,
        string? status,
        int pageSize)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new StudentListViewModel
        {
            Students = result.Select(s => new StudentListItemViewModel
            {
                Id = s.Id,
                StudentNumber = s.StudentNumber,
                UserName = s.UserName,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                IsActive = s.IsActive,
                CreatedOn = s.CreatedOn,
                CreatedByName = ResolveUserName(s.CreatedByName, s.CreatedBy),
                ModifiedOn = s.ModifiedOn,
                ModifiedByName = ResolveUserName(s.ModifiedByName, s.ModifiedBy),
            }).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            StudentNumber = studentNumber?.Trim() ?? string.Empty,
            FullName = fullName?.Trim() ?? string.Empty,
            UserName = userName?.Trim() ?? string.Empty,
            Email = email?.Trim() ?? string.Empty,
            Phone = phone?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("studentNumber", studentNumber),
                ("fullName", fullName),
                ("userName", userName),
                ("email", email),
                ("phone", phone),
                ("status", normalizedStatus)),
        };
    }

    public static StudentListViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? studentNumber = null,
        string? fullName = null,
        string? userName = null,
        string? email = null,
        string? phone = null,
        string? status = null)
    {
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new StudentListViewModel
        {
            Page = page,
            PageSize = pageSize,
            StudentNumber = studentNumber?.Trim() ?? string.Empty,
            FullName = fullName?.Trim() ?? string.Empty,
            UserName = userName?.Trim() ?? string.Empty,
            Email = email?.Trim() ?? string.Empty,
            Phone = phone?.Trim() ?? string.Empty,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("studentNumber", studentNumber),
                ("fullName", fullName),
                ("userName", userName),
                ("email", email),
                ("phone", phone),
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
