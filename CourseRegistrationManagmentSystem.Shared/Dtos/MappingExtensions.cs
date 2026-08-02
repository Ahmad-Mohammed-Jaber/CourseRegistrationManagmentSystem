using CourseRegistrationManagmentSystem.Shared.Models;

namespace CourseRegistrationManagmentSystem.Shared.Dtos;

public static class MappingExtensions
{
    public static UserDto? ToDto(this User user)
    {
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive
        };
    }

    public static StudentDto? ToDto(this Student student)
    {
        if (student == null) return null;

        return new StudentDto
        {
            Id = student.Id,
            UserName = student.UserName,
            FullName = student.FullName,
            Role = student.Role,
            IsActive = student.IsActive,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            Phone = student.Phone
        };
    }

    public static CourseDto? ToDto(this Course course)
    {
        if (course == null) return null;

        return new CourseDto
        {
            Id = course.Id,
            CourseCode = course.CourseCode,
            CourseName = course.CourseName,
            CreditHours = course.CreditHours,
            Description = course.Description,
            IsActive = course.IsActive
        };
    }

    public static ClassDto? ToDto(this Class cls)
    {
        if (cls == null) return null;

        return new ClassDto
        {
            Id = cls.Id,
            CourseId = cls.CourseId,
            ClassName = cls.ClassName,
            Instructor = cls.Instructor,
            Capacity = cls.Capacity,
            StartDate = cls.StartDate,
            EndDate = cls.EndDate,
            Schedule = cls.Schedule,
            IsActive = cls.IsActive
        };
    }

    public static RegistrationDto? ToDto(this Registrations reg)
    {
        if (reg == null) return null;

        return new RegistrationDto
        {
            Id = reg.Id,
            StudentId = reg.StudentId,
            ClassId = reg.ClassId,
            RegistrationDate = reg.RegsitrationDate,
            Status = reg.Status
        };
    }
}
