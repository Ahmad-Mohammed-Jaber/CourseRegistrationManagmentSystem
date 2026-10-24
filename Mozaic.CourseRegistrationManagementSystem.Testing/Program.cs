using Mozaic.CourseRegistrationManagementSystem.BL.Services;
using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Entities = Mozaic.CourseRegistrationManagementSystem.Shared.Entities;

// Smoke test for the unified read path: one GetAsync per entity, backed by
// one usp_Get<Entity> procedure that may or may not page.
//
// Run against a database seeded from SQL_Scripts/backup.sql:
//   dotnet run --project Mozaic.CourseRegistrationManagementSystem.Testing

static void DumpClasses(string label, PagedResult<Entities.Class> page)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine($"Page {page.PageNumber}/{page.TotalPages} size={page.PageSize} total={page.TotalCount} rows={page.Count} hasPrev={page.HasPreviousPage} hasNext={page.HasNextPage}");
    foreach (var c in page)
    {
        Console.WriteLine($"  {c.Id,4} {c.ClassName,-20} {c.Course?.CourseCode,-8} {c.Course?.CourseName,-28} instructor={c.Instructor,-18} active={c.IsActive,-5} seats={c.CurrentCapacity}/{c.MaxCapacity} createdBy='{c.CreatedByName}'");
    }
    Console.WriteLine();
}

static void DumpUsers(string label, PagedResult<Entities.User> page)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine($"Page {page.PageNumber}/{page.TotalPages} size={page.PageSize} total={page.TotalCount} rows={page.Count} hasPrev={page.HasPreviousPage} hasNext={page.HasNextPage}");
    foreach (var u in page)
    {
        Console.WriteLine($"  {u.Id,4} {u.UserName,-20} {u.FullName,-22} role={u.Role,-7} active={u.IsActive,-5} createdBy='{u.CreatedByName}'");
    }
    Console.WriteLine();
}

static void DumpStudents(string label, PagedResult<Entities.Student> page)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine($"Page {page.PageNumber}/{page.TotalPages} size={page.PageSize} total={page.TotalCount} rows={page.Count} hasPrev={page.HasPreviousPage} hasNext={page.HasNextPage}");
    foreach (var s in page)
    {
        Console.WriteLine($"  {s.Id,4} {s.UserName,-20} {s.FullName,-22} number={s.StudentNumber} active={s.IsActive,-5} createdBy='{s.CreatedByName}'");
    }
    Console.WriteLine();
}

static void DumpRegistrations(string label, PagedResult<Entities.Registration> page)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine($"Page {page.PageNumber}/{page.TotalPages} size={page.PageSize} total={page.TotalCount} rows={page.Count} hasPrev={page.HasPreviousPage} hasNext={page.HasNextPage}");
    foreach (var r in page)
    {
        Console.WriteLine($"  {r.Id,4} student='{r.Student.UserName}' class='{r.Class.ClassName}' course='{r.Course.CourseCode} {r.Course.CourseName}' status={r.Status}");
    }
    Console.WriteLine();
}

static void DumpCourses(string label, PagedResult<Entities.Course> page)
{
    Console.WriteLine($"--- {label} ---");
    Console.WriteLine($"Page {page.PageNumber}/{page.TotalPages} size={page.PageSize} total={page.TotalCount} rows={page.Count} hasPrev={page.HasPreviousPage} hasNext={page.HasNextPage}");
    foreach (var c in page)
    {
        Console.WriteLine($"  {c.Id,4} {c.CourseCode,-10} {c.CourseName,-28} hours={c.CreditHours} active={c.IsActive,-5} createdBy='{c.CreatedByName}'");
    }
    Console.WriteLine();
}

DumpClasses("classes: Active filter, page 1 size 3",
    await ClassService.GetAsync(statusFilter: ClassResult.StatusActive, pageNumber: 1, pageSize: 3));

DumpClasses("classes: Inactive filter",
    await ClassService.GetAsync(statusFilter: ClassResult.StatusInactive));

DumpClasses("classes: All filter",
    await ClassService.GetAsync(statusFilter: ClassResult.StatusAll));

DumpClasses("classes: search 'Dr.'",
    await ClassService.GetAsync(search: "Dr.", statusFilter: ClassResult.StatusAll));

DumpClasses("classes: search by course code",
    await ClassService.GetAsync(search: "CS101", statusFilter: ClassResult.StatusAll));

DumpClasses("classes: page past the end (clamps to the last page)",
    await ClassService.GetAsync(statusFilter: ClassResult.StatusAll, pageNumber: 99, pageSize: 3));

DumpClasses("classes: search with no match",
    await ClassService.GetAsync(search: "zzz-no-such-thing", statusFilter: ClassResult.StatusAll));

DumpUsers("users: no search, page 1 size 3",
    await UserService.GetAsync(pageNumber: 1, pageSize: 3));

DumpUsers("users: search 'admin'",
    await UserService.GetAsync(search: "admin"));

DumpUsers("users: page past the end (clamps to the last page)",
    await UserService.GetAsync(pageNumber: 99, pageSize: 3));

DumpStudents("students: all",
    await StudentService.GetAsync());

DumpStudents("students: by user id",
    await StudentService.GetAsync(userId: 2));

DumpRegistrations("registrations: all",
    await RegistrationService.GetAsync());

DumpRegistrations("registrations: by student id",
    await RegistrationService.GetAsync(studentId: 1));

DumpCourses("courses: all",
    await CourseService.GetAsync());

Console.WriteLine("Smoke test completed.");
