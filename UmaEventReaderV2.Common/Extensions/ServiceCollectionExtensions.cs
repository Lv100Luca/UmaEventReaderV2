using Microsoft.Extensions.DependencyInjection;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Services;

namespace UmaEventReaderV2.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventAreaOffsetProvider(
        this IServiceCollection services,
        Action<EventAreaOffsetOptions> configure)
    {
        return services
            .Configure(configure)
            .AddSingleton<EventAreaOffsetProvider>();
    }

    public static IServiceCollection AddDebugTextExtractor(
        this IServiceCollection services,
        Action<DebugTextExtractorOptions> options)
    {
        return services
            .Configure(options)
            .AddSingleton<ITextExtractor, DebugTextExtractor>();
    }

    public static IServiceCollection AddUmaEventReaderServices(
        this IServiceCollection services)
    {
        return services.AddSingleton<IUmaEventRepository, UmaEventMemoryRepository>()
            .AddSingleton<IUmaEventService, UmaEventService>()
            .AddSingleton<IScreenshotAreaProvider, StaticScreenshotAreaProvider>()
            .AddEventAreaOffsetProvider(options => options.Offset = 55)
            .AddSingleton<OcrService>()
            .AddSingleton<UmaEventReader>()
            .AddSingleton<IScreenshotProvider, ScreenshotProvider>()
#if DEBUG
            .AddSingleton<ITextExtractor, DebugTextExtractor>();
#else
            .AddSingleton<ITextExtractor, TesseractTextExtractor>();
#endif
    }
}