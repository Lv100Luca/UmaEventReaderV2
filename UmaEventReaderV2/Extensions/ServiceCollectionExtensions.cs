using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Services;

namespace UmaEventReaderV2.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUmaEventDbServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<IUmaEventRepository, UmaEventMemoryRepository>()
            .AddSingleton<IUmaEventService, UmaEventService>();
    }

    public static IServiceCollection AddScreenshotAreaProvider(this IServiceCollection services, ParserResult<ProgramArgs> args)
    {
        services.AddSingleton<EventAreaOffsetProvider>(_ => new EventAreaOffsetProvider { Offset = 55 });

        // if (args.Value.SelectArea)
            // return services.AddSingleton<SelectAreaOverlay>()
                // .AddSingleton<IScreenshotAreaProvider, ScreenshotAreaSelector>();

        return services.AddSingleton<IScreenshotAreaProvider, StaticScreenshotAreaProvider>();
    }
}