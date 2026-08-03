namespace CourseRegistrationManagmentSystem.Shared.Models;

public class Student : User
{
    new public Guid Id { get; set; }

    public Guid UserId { get; set; } = Guid.NewGuid();

    public int StudentNumber { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public Student()
    {
        Role = UserRoles.Student;
    }
}
