using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

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

    public string Status { get; set; } = "All";

    public Dictionary<string, string> RouteValues { get; set; } = new();

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static StudentListViewModel FromResult(
        StudentResult result,
        IMapper mapper,
        string? studentNumber,
        string? fullName,
        string? userName,
        string? email,
        string? phone,
        string? status,
        int pageSize)
    {
        return mapper.Map<StudentListViewModel>(
            new StudentListMappingSource(
                result, studentNumber, fullName, userName, email, phone, status, pageSize));
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
}
