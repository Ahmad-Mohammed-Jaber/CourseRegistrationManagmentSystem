using System.Collections;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

public class PagedResult<T> : IReadOnlyList<T>
{
    public List<T> Items { get; set; } = new();

    public int Count => Items.Count;

    public T this[int index] => Items[index];

    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int TotalCount { get; set; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static void Normalize(ref int pageNumber, ref int pageSize, int defaultPageSize = 20, int maxPageSize = 100)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize < 1)
        {
            pageSize = defaultPageSize;
        }

        if (pageSize > maxPageSize)
        {
            pageSize = maxPageSize;
        }
    }

    public static void Normalize(ref int? pageNumber, ref int? pageSize, int defaultPageSize = 20, int maxPageSize = 100)
    {
        if (!pageNumber.HasValue && !pageSize.HasValue)
        {
            return;
        }

        int page = pageNumber ?? 1;
        int size = pageSize ?? defaultPageSize;
        Normalize(ref page, ref size, defaultPageSize, maxPageSize);
        pageNumber = page;
        pageSize = size;
    }

    public static PagedResult<T> FromList(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        Normalize(ref pageNumber, ref pageSize);

        var list = source as IReadOnlyList<T> ?? source.ToList();
        return new PagedResult<T>
        {
            Items = list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = list.Count,
        };
    }
}
