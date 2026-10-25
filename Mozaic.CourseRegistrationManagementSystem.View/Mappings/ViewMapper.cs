using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mozaic.CourseRegistrationManagementSystem.View.Mappings;

public static class ViewMapper
{
    private static readonly MapperConfiguration Configuration =
        new(configuration => configuration.AddProfile<ViewMappingProfile>(), NullLoggerFactory.Instance);

    public static IMapper Instance { get; } = Configuration.CreateMapper();
}
