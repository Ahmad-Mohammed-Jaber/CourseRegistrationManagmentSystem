using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Enums;
using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.Shared.Session;
using Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Mappings;

public sealed class WebAppMappingProfile : Profile
{
    public WebAppMappingProfile()
    {
        MapUserListItems();
        MapStudentListItems();
        MapCourseListItems();
        MapClassListItems();
        MapRegistrationListItems();
        MapBrowseClassItems();
        MapFormModels();
        MapSelectListItems();
        MapListViewModels();

        CreateMap<User, UserSession>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ConstructUsing(s => new UserSession(s.Id, s.UserName, s.FullName, s.IsActive)
            {
                Role = s.Role,
            });
    }

    private void MapUserListItems()
    {
        CreateMap<User, UserListItemViewModel>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => DisplayUserName(s.CreatedByName, s.CreatedBy)))
            .ForMember(d => d.ModifiedByName, o => o.MapFrom(s => DisplayUserName(s.ModifiedByName, s.ModifiedBy)));
    }

    private void MapStudentListItems()
    {
        CreateMap<Student, StudentListItemViewModel>()
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => DisplayUserName(s.CreatedByName, s.CreatedBy)))
            .ForMember(d => d.ModifiedByName, o => o.MapFrom(s => DisplayUserName(s.ModifiedByName, s.ModifiedBy)));
    }

    private void MapCourseListItems()
    {
        CreateMap<Course, CourseListItemViewModel>()
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => DisplayUserName(s.CreatedByName, s.CreatedBy)))
            .ForMember(d => d.ModifiedByName, o => o.MapFrom(s => DisplayUserName(s.ModifiedByName, s.ModifiedBy)));
    }

    private void MapClassListItems()
    {
        CreateMap<Class, ClassListItemViewModel>()
            .ForMember(d => d.CourseName, o => o.MapFrom(s => CourseLabel(s.Course)))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => ScheduleHelper.GetScheduleString(s.Schedule)))
            .ForMember(d => d.Seats, o => o.MapFrom(s => $"{s.CurrentCapacity}/{s.MaxCapacity}"))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => DisplayUserName(s.CreatedByName, s.CreatedBy)))
            .ForMember(d => d.ModifiedByName, o => o.MapFrom(s => DisplayUserName(s.ModifiedByName, s.ModifiedBy)));
    }

    private void MapRegistrationListItems()
    {
        CreateMap<Registration, RegistrationListItemViewModel>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => StudentLabel(s.Student)))
            .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course == null ? string.Empty : s.Course.CourseName))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => s.Class == null ? string.Empty : s.Class.ClassName))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => DisplayUserName(s.CreatedByName, s.CreatedBy)))
            .ForMember(d => d.ModifiedByName, o => o.MapFrom(s => DisplayUserName(s.ModifiedByName, s.ModifiedBy)));

        CreateMap<Registration, MyRegistrationViewModel>()
            .ForMember(d => d.RegistrationId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ClassId, o => o.MapFrom(s => s.Class == null ? s.ClassId : s.Class.Id))
            .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course == null ? string.Empty : s.Course.CourseName))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => s.Class == null ? string.Empty : s.Class.ClassName))
            .ForMember(d => d.Instructor, o => o.MapFrom(s => s.Class == null ? string.Empty : s.Class.Instructor))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => s.Class == null
                ? string.Empty
                : ScheduleHelper.GetScheduleString(s.Class.Schedule)))
            .ForMember(d => d.StartDate, o => o.MapFrom(s => s.Class == null ? default : s.Class.StartDate))
            .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Class == null ? default : s.Class.EndDate));
    }

    private void MapBrowseClassItems()
    {
        CreateMap<Class, BrowseClassViewModel>()
            .ForMember(d => d.ClassId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course == null ? string.Empty : s.Course.CourseName))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => ScheduleHelper.GetScheduleString(s.Schedule)));
    }

    private void MapFormModels()
    {
        CreateMap<Course, CourseFormViewModel>();

        CreateMap<CourseFormViewModel, Course>()
            .ForMember(d => d.CourseCode, o => o.MapFrom(s => s.CourseCode.Trim()))
            .ForMember(d => d.CourseName, o => o.MapFrom(s => s.CourseName.Trim()))
            .ForMember(d => d.Description, o => o.MapFrom(s => s.Description == null ? string.Empty : s.Description.Trim()))
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.CreatedByName, o => o.Ignore())
            .ForMember(d => d.ModifiedByName, o => o.Ignore());

        CreateMap<Class, ClassFormViewModel>()
            .ForMember(d => d.Courses, o => o.Ignore())
            .ForMember(d => d.Sunday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Sunday)))
            .ForMember(d => d.Monday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Monday)))
            .ForMember(d => d.Tuesday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Tuesday)))
            .ForMember(d => d.Wednesday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Wednesday)))
            .ForMember(d => d.Thursday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Thursday)))
            .ForMember(d => d.Friday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Friday)))
            .ForMember(d => d.Saturday, o => o.MapFrom(s => s.Schedule.HasFlag(DaysOfTheWeek.Saturday)));

        CreateMap<ClassFormViewModel, Class>()
            .ForMember(d => d.ClassName, o => o.MapFrom(s => s.ClassName.Trim()))
            .ForMember(d => d.Instructor, o => o.MapFrom(s => s.Instructor.Trim()))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => s.ToSchedule()))
            .ForMember(d => d.IsRegistered, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.CreatedByName, o => o.Ignore())
            .ForMember(d => d.ModifiedByName, o => o.Ignore());

        CreateMap<User, EditUserViewModel>()
            .ForMember(d => d.Role, o => o.MapFrom(s => s.Role.ToString()));

        CreateMap<EditUserViewModel, User>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName.Trim()))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName.Trim()))
            // Role and password are not editable through this form; the service
            // preserves the stored password and existing role handling.
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.CreatedByName, o => o.Ignore())
            .ForMember(d => d.ModifiedByName, o => o.Ignore());

        CreateMap<Student, EditStudentViewModel>();

        CreateMap<EditStudentViewModel, Student>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName.Trim()))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName.Trim()))
            .ForMember(d => d.Email, o => o.MapFrom(s => s.Email.Trim()))
            .ForMember(d => d.Phone, o => o.MapFrom(s => s.Phone.Trim()))
            .ForMember(d => d.UserId, o => o.Ignore())
            .ForMember(d => d.Role, o => o.Ignore())
            .ForMember(d => d.PasswordHash, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.CreatedByName, o => o.Ignore())
            .ForMember(d => d.ModifiedByName, o => o.Ignore());

        CreateMap<Registration, RegistrationFormViewModel>()
            .ForMember(d => d.Status, o => o.MapFrom(s => string.IsNullOrWhiteSpace(s.Status) ? "Registered" : s.Status))
            .ForMember(d => d.Students, o => o.Ignore())
            .ForMember(d => d.Classes, o => o.Ignore())
            .ForMember(d => d.Statuses, o => o.Ignore());

        CreateMap<RegistrationFormViewModel, Registration>()
            .ForMember(d => d.Class, o => o.Ignore())
            .ForMember(d => d.Student, o => o.Ignore())
            .ForMember(d => d.Course, o => o.Ignore())
            .ForMember(d => d.CreatedOn, o => o.Ignore())
            .ForMember(d => d.ModifiedOn, o => o.Ignore())
            .ForMember(d => d.CreatedBy, o => o.Ignore())
            .ForMember(d => d.ModifiedBy, o => o.Ignore())
            .ForMember(d => d.CreatedByName, o => o.Ignore())
            .ForMember(d => d.ModifiedByName, o => o.Ignore());
    }

    private void MapListViewModels()
    {
        CreateMap<UserListMappingSource, UserListViewModel>()
            .ForMember(d => d.Users, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.UserName, o => o.MapFrom(s => Trimmed(s.UserName)))
            .ForMember(d => d.FullName, o => o.MapFrom(s => Trimmed(s.FullName)))
            .ForMember(d => d.Role, o => o.MapFrom(s => NormalizeAll(s.Role)))
            .ForMember(d => d.Status, o => o.MapFrom(s => NormalizeAll(s.Status)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => UserRouteValues(s)));

        CreateMap<StudentListMappingSource, StudentListViewModel>()
            .ForMember(d => d.Students, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.StudentNumber, o => o.MapFrom(s => Trimmed(s.StudentNumber)))
            .ForMember(d => d.FullName, o => o.MapFrom(s => Trimmed(s.FullName)))
            .ForMember(d => d.UserName, o => o.MapFrom(s => Trimmed(s.UserName)))
            .ForMember(d => d.Email, o => o.MapFrom(s => Trimmed(s.Email)))
            .ForMember(d => d.Phone, o => o.MapFrom(s => Trimmed(s.Phone)))
            .ForMember(d => d.Status, o => o.MapFrom(s => NormalizeAll(s.Status)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => StudentRouteValues(s)));

        CreateMap<CourseListMappingSource, CourseListViewModel>()
            .ForMember(d => d.Courses, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.Code, o => o.MapFrom(s => Trimmed(s.Code)))
            .ForMember(d => d.Name, o => o.MapFrom(s => Trimmed(s.Name)))
            .ForMember(d => d.Description, o => o.MapFrom(s => Trimmed(s.Description)))
            .ForMember(d => d.Status, o => o.MapFrom(s => NormalizeAll(s.Status)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => CourseRouteValues(s)));

        CreateMap<ClassListMappingSource, ClassListViewModel>()
            .ForMember(d => d.Classes, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => Trimmed(s.ClassName)))
            .ForMember(d => d.Instructor, o => o.MapFrom(s => Trimmed(s.Instructor)))
            .ForMember(d => d.Course, o => o.MapFrom(s => Trimmed(s.Course)))
            .ForMember(d => d.Status, o => o.MapFrom(s => NormalizeClassStatus(s.Status)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => ClassRouteValues(s)));

        CreateMap<RegistrationListMappingSource, RegistrationListViewModel>()
            .ForMember(d => d.Registrations, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.Student, o => o.MapFrom(s => Trimmed(s.Student)))
            .ForMember(d => d.Course, o => o.MapFrom(s => Trimmed(s.Course)))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => Trimmed(s.ClassName)))
            .ForMember(d => d.Status, o => o.MapFrom(s => NormalizeAll(s.Status)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => RegistrationRouteValues(s)));

        CreateMap<MyRegistrationsListMappingSource, MyRegistrationsListViewModel>()
            .ForMember(d => d.Registrations, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.Course, o => o.MapFrom(s => Trimmed(s.Course)))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => Trimmed(s.ClassName)))
            .ForMember(d => d.Status, o => o.MapFrom(s => NormalizeAll(s.Status)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => MyRegistrationsRouteValues(s)));

        CreateMap<BrowseClassesMappingSource, BrowseClassesViewModel>()
            .ForMember(d => d.Classes, o => o.MapFrom(s => s.Result.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => CurrentPage(s.Result.PageNumber, s.Result.TotalPages)))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.Result.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.Result.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => TotalPages(s.Result.TotalPages)))
            .ForMember(d => d.OpenSeatsCount, o => o.MapFrom(s => s.Result.OpenSeatsCount))
            .ForMember(d => d.MyRegistrationsCount, o => o.MapFrom(s => s.Result.MyRegistrationsCount))
            .ForMember(d => d.Course, o => o.MapFrom(s => Trimmed(s.Course)))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => Trimmed(s.ClassName)))
            .ForMember(d => d.Instructor, o => o.MapFrom(s => Trimmed(s.Instructor)))
            .ForMember(d => d.RouteValues, o => o.MapFrom(s => BrowseClassesRouteValues(s)));
    }

    private void MapSelectListItems()
    {        CreateMap<Course, SelectListItem>()
            .ForMember(d => d.Text, o => o.MapFrom(s => CourseLabel(s)))
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.Selected, o => o.MapFrom(_ => false))
            .ForMember(d => d.Disabled, o => o.MapFrom(_ => false))
            .ForMember(d => d.Group, o => o.Ignore());

        CreateMap<Student, SelectListItem>()
            .ForMember(d => d.Text, o => o.MapFrom(s => StudentLabel(s)))
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.Selected, o => o.MapFrom(_ => false))
            .ForMember(d => d.Disabled, o => o.MapFrom(_ => false))
            .ForMember(d => d.Group, o => o.Ignore());

        CreateMap<Class, SelectListItem>()
            .ForMember(d => d.Text, o => o.MapFrom(s => s.ClassName))
            .ForMember(d => d.Value, o => o.MapFrom(s => s.Id.ToString()))
            .ForMember(d => d.Selected, o => o.MapFrom(_ => false))
            .ForMember(d => d.Disabled, o => o.MapFrom(_ => false))
            .ForMember(d => d.Group, o => o.Ignore());
    }

    private static string DisplayUserName(string? name, int userId)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        return userId <= 0 ? "—" : $"#{userId}";
    }

    private static string CourseLabel(Course? course)
    {
        return $"{course?.CourseCode} {course?.CourseName}".Trim();
    }

    private static string StudentLabel(Student? student)
    {
        return $"{student?.FullName} ({student?.UserName})";
    }

    private static int CurrentPage(int pageNumber, int totalPages)
    {
        return Math.Min(pageNumber, Math.Max(1, totalPages));
    }

    private static int TotalPages(int totalPages)
    {
        return Math.Max(1, totalPages);
    }

    private static string Trimmed(string? value)
    {
        return value?.Trim() ?? string.Empty;
    }

    private static string NormalizeAll(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "All" : value!;
    }

    private static string NormalizeClassStatus(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? ClassResult.StatusActive : value!;
    }

    private static Dictionary<string, string> ClassRouteValues(ClassListMappingSource source)
    {
        var status = NormalizeClassStatus(source.Status);
        var values = ListRouteValues.Create(
            source.PageSize,
            ("className", source.ClassName),
            ("instructor", source.Instructor),
            ("course", source.Course),
            ("status", status));
        values["status"] = status;
        return values;
    }

    private static Dictionary<string, string> UserRouteValues(UserListMappingSource source)
    {
        return ListRouteValues.Create(
            source.PageSize,
            ("userName", source.UserName),
            ("fullName", source.FullName),
            ("role", NormalizeAll(source.Role)),
            ("status", NormalizeAll(source.Status)));
    }

    private static Dictionary<string, string> StudentRouteValues(StudentListMappingSource source)
    {
        return ListRouteValues.Create(
            source.PageSize,
            ("studentNumber", source.StudentNumber),
            ("fullName", source.FullName),
            ("userName", source.UserName),
            ("email", source.Email),
            ("phone", source.Phone),
            ("status", NormalizeAll(source.Status)));
    }

    private static Dictionary<string, string> CourseRouteValues(CourseListMappingSource source)
    {
        return ListRouteValues.Create(
            source.PageSize,
            ("code", source.Code),
            ("name", source.Name),
            ("description", source.Description),
            ("status", NormalizeAll(source.Status)));
    }

    private static Dictionary<string, string> RegistrationRouteValues(RegistrationListMappingSource source)
    {
        return ListRouteValues.Create(
            source.PageSize,
            ("student", source.Student),
            ("course", source.Course),
            ("className", source.ClassName),
            ("status", NormalizeAll(source.Status)));
    }

    private static Dictionary<string, string> MyRegistrationsRouteValues(MyRegistrationsListMappingSource source)
    {
        return ListRouteValues.Create(
            source.PageSize,
            ("course", source.Course),
            ("className", source.ClassName),
            ("status", NormalizeAll(source.Status)));
    }

    private static Dictionary<string, string> BrowseClassesRouteValues(BrowseClassesMappingSource source)
    {
        return ListRouteValues.Create(
            source.PageSize,
            ("course", source.Course),
            ("className", source.ClassName),
            ("instructor", source.Instructor));
    }
}
