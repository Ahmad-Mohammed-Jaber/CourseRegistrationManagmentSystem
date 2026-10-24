using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Strongly-typed page model for Views/Users/Index.cshtml.
/// Holds the current page of users plus the pager state and the search term.
/// </summary>
public sealed class UserListViewModel
{
    public List<UserListItemViewModel> Users { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    /// <summary>The active search term, echoed back into the search box.</summary>
    public string Query { get; set; } = string.Empty;

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static UserListViewModel FromResult(UserResult result, string? query)
    {
        return new UserListViewModel
        {
            Users = result.Select(u => new UserListItemViewModel
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                CreatedOn = u.CreatedOn,
                CreatedByName = ResolveUserName(u.CreatedByName, u.CreatedBy),
                ModifiedOn = u.ModifiedOn,
                ModifiedByName = ResolveUserName(u.ModifiedByName, u.ModifiedBy),
            }).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            Query = query?.Trim() ?? string.Empty,
        };
    }

    public static UserListViewModel Empty(int page = 1, int pageSize = 20, string? query = null)
    {
        return new UserListViewModel { Page = page, PageSize = pageSize, Query = query?.Trim() ?? string.Empty };
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
