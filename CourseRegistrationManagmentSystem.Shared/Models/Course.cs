namespace CourseRegistrationManagmentSystem.Shared.Models;

public class Course
{
    public Guid Id { get; set; }

    public string CourseCode { get; set; }

    public string CourseName { get; set; }

    public double CreditHours { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }
}
