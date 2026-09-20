using Shared.Entities;

namespace Shared.Dtos;

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

    public static StudentDto? ToDto(this Student student, User user)
    {
        if (student == null || user == null) return null;

        return new StudentDto
        {
            Id =  student.Id,
            UserId = student.UserId,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            Phone = student.Phone,
            UserName = user.UserName,
            FullName = user.FullName,
            IsActive = user.IsActive,
            Role = user.Role
        };
    }

    public static StudentDto? ToDto(this Student student)
    {
        if (student == null) return null;

        return new StudentDto
        {
            Id = student.Id,
            UserId = student.UserId,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            Phone = student.Phone,
            UserName = student.UserName,
            FullName = student.FullName,
            IsActive = student.IsActive,
            Role = student.Role
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
            MaxCapacity = cls.MaxCapacity,
            CurrentCapacity = cls.CurrentCapacity,
            StartDate = cls.StartDate,
            EndDate = cls.EndDate,
            Schedule = cls.Schedule,
            IsActive = cls.IsActive
        };
    }

    public static User ToEntity(this UserDto dto)
    {
        return new User
        {
            Id = dto.Id,
            UserName = dto.UserName,
            FullName = dto.FullName,
            Role = dto.Role,
            IsActive = dto.IsActive
        };
    }

    public static Student ToEntity(this StudentDto dto)
    {
        return new Student
        {
            Id = dto.Id,
            UserId = dto.UserId,
            StudentNumber = dto.StudentNumber,
            Email = dto.Email,
            Phone = dto.Phone,
            UserName = dto.UserName,
            FullName = dto.FullName,
            IsActive = dto.IsActive,
            Role = dto.Role
        };
    }

    public static Class ToEntity(this ClassDto dto)
    {
        return new Class
        {
            Id = dto.Id,
            CourseId = dto.CourseId,
            ClassName = dto.ClassName,
            Instructor = dto.Instructor,
            MaxCapacity = dto.MaxCapacity,
            CurrentCapacity = dto.CurrentCapacity,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Schedule = dto.Schedule,
            IsActive = dto.IsActive
        };
    }

    public static Course ToEntity(this CourseDto dto)
    {
        return new Course
        {
            Id = dto.Id,
            CourseCode = dto.CourseCode,
            CourseName = dto.CourseName,
            CreditHours = dto.CreditHours,
            Description = dto.Description,
            IsActive = dto.IsActive
        };
    }

    public static RegistrationDto? ToDto(this Registration registration)
    {
        if (registration == null) return null;

        return new RegistrationDto
        {
            Id = registration.Id,
            StudentId = registration.StudentId,
            ClassId = registration.ClassId,
            RegistrationDate = registration.RegsitrationDate,
            Status = registration.Status
        };
    }

    public static Registration ToEntity(this RegistrationDto registrationDto)
    {
        return new Registration
        {
            Id = registrationDto.Id,
            StudentId = registrationDto.StudentId,
            ClassId = registrationDto.ClassId,
            RegsitrationDate = registrationDto.RegistrationDate,
            Status = registrationDto.Status
        };
    }


    public static RegistrationDetailsDto ToDetailsDto(
         this Registration registration,
         Class cls)
    {
        return new RegistrationDetailsDto
        {
            RegistrationId = registration.Id,
            ClassName = cls.ClassName,
            Instructor = cls.Instructor,
            Schedule = cls.Schedule.ToString(),
            StartDate = cls.StartDate,
            EndDate = cls.EndDate,
            RegistrationDate = registration.RegsitrationDate,
            Status = registration.Status
        };
    }
}
