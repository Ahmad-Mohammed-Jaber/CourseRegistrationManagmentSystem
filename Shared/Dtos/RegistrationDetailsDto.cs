namespace Shared.Dtos;

public class RegistrationDetailsDto
{
    public int RegistrationId { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public string Instructor { get; set; } = string.Empty;

    public string Schedule { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public DateTime RegistrationDate { get; set; }

    public string Status { get; set; } = string.Empty;
}