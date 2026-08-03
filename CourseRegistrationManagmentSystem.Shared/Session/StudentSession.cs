namespace CourseRegistrationManagmentSystem.Shared.Session;

public record StudentSession(Guid Id, Guid UserId, string UserName, int StudentNumber, string Email, string Phone);

