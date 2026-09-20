namespace Shared.Dtos;

public class RegistrationDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public string? StudentUserName { get; set; }
    public int ClassId { get; init; }
    public string? ClassName { get; set; }
    public string? CourseName { get; set; }
    public DateTime RegistrationDate { get; init; }
    public required string Status { get; set; }
}