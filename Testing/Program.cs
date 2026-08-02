using CourseRegistrationManagmentSystem.Data.Repository;

UserRepository userRepo = new UserRepository();

await userRepo.AddAsync(new User
{
    Id = Guid.NewGuid(),
    FullName = "Test",
    IsActive = true,
    PasswordHash = "Test",
    Role = User.UserRoles.Student,
    UserName = "Test",
});