namespace Shared.DTOs;

public record StudentProfile(
    int Id,
    string UserName,
    string FullName,
    string Role,
    int StudentNumber,
    string Email,
    string Phone
);
