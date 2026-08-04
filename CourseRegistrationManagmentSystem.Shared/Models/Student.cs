namespace CourseRegistrationManagmentSystem.Shared.Models;

public class Student : User
{
    new public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int StudentNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
