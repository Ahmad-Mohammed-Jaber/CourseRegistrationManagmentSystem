namespace CourseRegistrationManagmentSystem.Models;

public class Student
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int StudentNumber { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }
}
