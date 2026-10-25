using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

public sealed record UserListMappingSource(
    UserResult Result,
    string? UserName,
    string? FullName,
    string? Role,
    string? Status,
    int PageSize);

public sealed record StudentListMappingSource(
    StudentResult Result,
    string? StudentNumber,
    string? FullName,
    string? UserName,
    string? Email,
    string? Phone,
    string? Status,
    int PageSize);

public sealed record CourseListMappingSource(
    CourseResult Result,
    string? Code,
    string? Name,
    string? Description,
    string? Status,
    int PageSize);

public sealed record ClassListMappingSource(
    ClassResult Result,
    string? ClassName,
    string? Instructor,
    string? Course,
    string? Status,
    int PageSize);

public sealed record RegistrationListMappingSource(
    RegistrationResult Result,
    string? Student,
    string? Course,
    string? ClassName,
    string? Status,
    int PageSize);

public sealed record MyRegistrationsListMappingSource(
    RegistrationResult Result,
    string? Course,
    string? ClassName,
    string? Status,
    int PageSize);

public sealed record BrowseClassesMappingSource(
    ClassResult Result,
    string? Course,
    string? ClassName,
    string? Instructor,
    int PageSize);
