namespace CourseRegistrationManagmentSystem.Shared.Models;

public class Registration
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public Guid ClassId { get; set; }

    public DateTime RegsitrationDate { get; set; }

    public string Status { get; set; }

}
