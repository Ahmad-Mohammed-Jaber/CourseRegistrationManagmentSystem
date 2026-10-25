using AutoMapper;
using Mozaic.CourseRegistrationManagementSystem.Shared.Entities;
using Mozaic.CourseRegistrationManagementSystem.Shared.Helpers;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.View.Models;

namespace Mozaic.CourseRegistrationManagementSystem.View.Mappings;

public sealed class ViewMappingProfile : Profile
{
    public ViewMappingProfile()
    {
        CreateMap<Class, ClassBrowseRow>()
            .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course == null ? string.Empty : s.Course.CourseName))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => ScheduleHelper.GetScheduleString(s.Schedule)));

        CreateMap<Class, ClassGridRow>()
            .ForMember(d => d.Schedule, o => o.MapFrom(s => ScheduleHelper.GetScheduleString(s.Schedule)));

        CreateMap<Registration, RegistrationGridRow>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student == null ? string.Empty : s.Student.UserName))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => s.Class == null ? string.Empty : s.Class.ClassName))
            .ForMember(d => d.CourseName, o => o.MapFrom(s => s.Course == null ? string.Empty : s.Course.CourseName));

        CreateMap<Registration, MyRegistrationGridRow>()
            .ForMember(d => d.RegistrationId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.ClassName, o => o.MapFrom(s => s.Class == null ? string.Empty : s.Class.ClassName))
            .ForMember(d => d.Instructor, o => o.MapFrom(s => s.Class == null ? string.Empty : s.Class.Instructor))
            .ForMember(d => d.Schedule, o => o.MapFrom(s => s.Class == null
                ? string.Empty
                : ScheduleHelper.GetScheduleString(s.Class.Schedule)))
            .ForMember(d => d.StartDate, o => o.MapFrom(s => s.Class == null ? default : s.Class.StartDate))
            .ForMember(d => d.EndDate, o => o.MapFrom(s => s.Class == null ? default : s.Class.EndDate));

        CreateMap<ClassResult, BrowseClassesViewModel>()
            .ForMember(d => d.Rows, o => o.MapFrom(s => s.Items))
            .ForMember(d => d.Page, o => o.MapFrom(s => Math.Min(s.PageNumber, Math.Max(1, s.TotalPages))))
            .ForMember(d => d.PageSize, o => o.MapFrom(s => s.PageSize))
            .ForMember(d => d.TotalCount, o => o.MapFrom(s => s.TotalCount))
            .ForMember(d => d.TotalPages, o => o.MapFrom(s => Math.Max(1, s.TotalPages)))
            .ForMember(d => d.OpenSeatsCount, o => o.MapFrom(s => s.OpenSeatsCount))
            .ForMember(d => d.MyRegistrationsCount, o => o.MapFrom(s => s.MyRegistrationsCount));
    }
}
