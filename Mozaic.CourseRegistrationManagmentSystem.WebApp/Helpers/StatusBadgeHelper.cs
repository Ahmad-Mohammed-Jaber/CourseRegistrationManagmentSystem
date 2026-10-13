namespace Mozaic.CourseRegistrationManagementSystem.WebApp;

public static class StatusBadgeHelper
{
    public static string GetBadgeClass(string? status) =>
        status?.Trim().ToLowerInvariant() switch
        {
            "registered" or "confirmed" or "open" or "active" => "bg-success",
            "pending" or "full" => "bg-warning text-dark",
            "cancelled" or "canceled" or "dropped" => "bg-danger",
            "inactive" => "bg-secondary",
            _ => "bg-secondary",
        };
}
