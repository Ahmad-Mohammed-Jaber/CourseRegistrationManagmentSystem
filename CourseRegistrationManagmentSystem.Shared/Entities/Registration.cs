namespace Shared.Entities;

public class Registration
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int ClassId { get; set; }

    public DateTime RegsitrationDate { get; set; }

    public string Status { get; set; } = string.Empty;
}