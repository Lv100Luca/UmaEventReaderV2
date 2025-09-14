using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Infrastructure;
using UmaEventReaderV2.Services;
using UmaEventReaderVs.WinForms;

namespace UmaEventReaderV2.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUmaEventDbServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<UmaDbContext>()
            .AddSingleton<IUmaEventRepository, UmaEventRepository>()
            .AddSingleton<IUmaEventService, UmaEventService>();
    }

    public static IServiceCollection AddScreenshotAreaProvider(this IServiceCollection services, ParserResult<ProgramArgs> args)
    {
        if (args.Value.SelectArea)
            return services.AddSingleton<SelectAreaOverlay>()
                .AddSingleton<EventAreaOffsetProvider>(_ => new EventAreaOffsetProvider { Offset = 60 })
                .AddSingleton<IScreenshotAreaProvider, ScreenshotAreaSelector>();

        return services.AddSingleton<IScreenshotAreaProvider, StaticScreenshotAreaProvider>();
    }
}