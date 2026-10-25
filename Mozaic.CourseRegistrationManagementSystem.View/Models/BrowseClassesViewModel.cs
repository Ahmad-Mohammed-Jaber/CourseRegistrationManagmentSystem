using Mozaic.CourseRegistrationManagementSystem.Shared.Results;
using Mozaic.CourseRegistrationManagementSystem.View.Mappings;

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
        return ViewMapper.Instance.Map<BrowseClassesViewModel>(result);
    }

    public static BrowseClassesViewModel Empty(int page = 1, int pageSize = 20)
    {
        return new BrowseClassesViewModel { Page = page, PageSize = pageSize };
    }
}
