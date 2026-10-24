namespace Mozaic.CourseRegistrationManagmentSystem.WebApp.Models;

/// <summary>
/// Builds the query-string values carried by a list pager. Empty filters and
/// the "All" pseudo-value are omitted so the URL stays readable.
/// </summary>
internal static class ListRouteValues
{
    public static Dictionary<string, string> Create(int pageSize, params (string Key, string? Value)[] filters)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["pageSize"] = pageSize.ToString(),
        };

        foreach (var (key, value) in filters)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            var trimmed = value.Trim();
            if (string.Equals(trimmed, "All", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            values[key] = trimmed;
        }

        return values;
    }
}
