using Mozaic.CourseRegistrationManagementSystem.Shared.Results;

namespace Mozaic.CourseRegistrationManagementSystem.View.Models;

public sealed class BrowseClassesViewModel
{
    public List<ClassBrowseRow> Rows { get; set; } = new();

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages { get; set; } = 1;

    public int OpenSeatsCount { get; set; }

    public int MyRegistrationsCount { get; set; }

    public bool HasPrevious => Page > 1;

    public bool HasNext => Page < TotalPages;

    public static BrowseClassesViewModel FromResult(ClassResult result)
    {
        return new BrowseClassesViewModel
        {
            Rows = result.Select(c => new ClassBrowseRow(c)).ToList(),
            Page = Math.Min(result.PageNumber, Math.Max(1, result.TotalPages)),
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = Math.Max(1, result.TotalPages),
            OpenSeatsCount = result.OpenSeatsCount,
            MyRegistrationsCount = result.MyRegistrationsCount,
        };
    }

    public static BrowseClassesViewModel Empty(int page = 1, int pageSize = 20)
    {
        return new BrowseClassesViewModel { Page = page, PageSize = pageSize };
    }
}
