using System.Collections;

namespace Mozaic.CourseRegistrationManagementSystem.Shared.Results;

/// <summary>
/// Generic paginated result. Replaces entity-specific page wrappers
/// (e.g. the old ClassBrowsePage) so any entity can be paginated
/// as PagedResult&lt;Course&gt;, PagedResult&lt;Student&gt;, etc.
/// This is a transport/DTO type, not a database entity.
///
/// The result *is* the list of rows: it implements IReadOnlyList&lt;T&gt;, so
/// an unpaged read can be used directly as a collection. Items stays
/// available for the callers that want the page metadata alongside the rows.
/// </summary>
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

    /// <summary>
    /// Normalizes the paging inputs of a read that may or may not be paged.
    /// Both stay null when no page was asked for, which means "every matching
    /// row"; otherwise the page number and size are clamped to the allowed range.
    /// </summary>
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

    /// <summary>
    /// Builds a page from an already-fetched sequence (e.g. rows from an
    /// existing stored procedure). Paging happens in memory, so this works
    /// with any SP shape — no paged-proc support required. Preserves source order.
    /// </summary>
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
