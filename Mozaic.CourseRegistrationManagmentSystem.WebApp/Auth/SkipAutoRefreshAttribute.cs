using Microsoft.AspNetCore.Mvc.Filters;

namespace Mozaic.CourseRegistrationManagementSystem.WebApp.Auth;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SkipAutoRefreshAttribute : Attribute, IFilterMetadata
{
}
