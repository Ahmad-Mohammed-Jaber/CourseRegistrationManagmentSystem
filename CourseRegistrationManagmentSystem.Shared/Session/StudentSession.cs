namespace CourseRegistrationManagmentSystem.Shared.Session;

public record StudentSession(Guid Id, Guid StudentId, int StudentNumber, string FullName, string Email, string Phone);
