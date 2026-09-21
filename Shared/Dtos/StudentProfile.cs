namespace Shared.DTOs;

public record StudentProfile(
    int StudentId,
    int UserId,
    string UserName,
    string FullName,
    string Role,
    int StudentNumber,
    string Email,
    string Phone
)
{
    // Backward compatibility: legacy single-Id consumers meant the Student PK.
    public int Id => StudentId;
};
