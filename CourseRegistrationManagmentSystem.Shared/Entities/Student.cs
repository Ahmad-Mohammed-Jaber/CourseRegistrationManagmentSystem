namespace Shared.Entities;

public class Student : User
{
    public new int Id { get; set; }

    public int UserId { get; set; }

    public int StudentNumber { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;
}