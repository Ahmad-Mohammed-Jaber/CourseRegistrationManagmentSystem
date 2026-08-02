namespace CourseRegistrationManagmentSystem.Shared.Session;

public record StudentSession(Guid Id, Guid UserId, int StudentNumber, string FullName, string Email, string Phone);

