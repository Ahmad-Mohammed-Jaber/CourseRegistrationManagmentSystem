using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

public sealed class UserListViewModel
{
    public List<UserListItemViewModel> Users { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public string UserName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string Role { get; set; } = "All";

    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static UserListViewModel FromResult(
        UserResult result,
        IMapper mapper,
        string? userName,
        string? fullName,
        string? role,
        string? status,
        int pageSize)
    {
        return mapper.Map<UserListViewModel>(
            new UserListMappingSource(result, userName, fullName, role, status, pageSize));
    }

    public static UserListViewModel Empty(
        int page = 1,
        int pageSize = 20,
        string? userName = null,
        string? fullName = null,
        string? role = null,
        string? status = null)
    {
        var normalizedRole = string.IsNullOrWhiteSpace(role) ? "All" : role!;
        var normalizedStatus = string.IsNullOrWhiteSpace(status) ? "All" : status!;

        return new UserListViewModel
        {
            Page = page,
            PageSize = pageSize,
            UserName = userName?.Trim() ?? string.Empty,
            FullName = fullName?.Trim() ?? string.Empty,
            Role = normalizedRole,
            Status = normalizedStatus,
            RouteValues = ListRouteValues.Create(
                pageSize,
                ("userName", userName),
                ("fullName", fullName),
                ("role", normalizedRole),
                ("status", normalizedStatus)),
        };
    }
}
