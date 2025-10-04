using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Services;
using UmaEventReaderV2.Services.Emitters;

namespace UmaEventReaderV2.Web.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers event area offset provider with options.
    /// </summary>
    public static IServiceCollection AddEventAreaOffsetProvider(
        this IServiceCollection services,
        Action<EventAreaOffsetOptions> configure)
    {
        return services
            .Configure(configure)
            .AddSingleton<EventAreaOffsetProvider>();
    }

    /// <summary>
    /// Registers a debug text extractor with predefined options.
    /// Only used in DEBUG builds.
    /// </summary>
    public static IServiceCollection AddDebugTextExtractor(
        this IServiceCollection services,
        Action<DebugTextExtractorOptions> options)
    {
        return services
            .Configure(options)
            .AddSingleton<ITextExtractor, DebugTextExtractor>();
    }

    /// <summary>
    /// Registers all Uma Event Reader core services.
    /// </summary>
    public static IServiceCollection AddUmaEventReaderServices(
        this IServiceCollection services)
    {
        // Infrastructure / Providers
        services
            .AddSingleton<IUmaEventJsonProvider, PlaywrightUmaEventJsonProvider>()
            // Alternative provider (for offline testing)
            // .AddSingleton<IUmaEventJsonProvider, StaticUmaEventJsonProvider>()
            .AddSingleton<IScreenshotProvider, ScreenshotProvider>()
            .AddSingleton<IScreenshotAreaProvider, StaticScreenshotAreaProvider>()
            .AddEventAreaOffsetProvider(options => options.Offset = 55);

        // Repositories
        services
            .AddSingleton<IUmaRepository, UmaRepository>()
            .AddSingleton<IUmaEventRepository, UmaEventMemoryRepository>()
            .AddSingleton<IRepositoryInitializer, RepositoryInitializer>();

        // Core Services
        services
            .AddSingleton<UmaReaderSettingsProvider>()
            .AddSingleton<UmaEventMapper>()
            .AddSingleton<IEventEmitter, CachingEventEmitter>()
            .AddSingleton<OcrService>()
            .AddSingleton<UmaEventReader>();

        // Text Extraction (debug vs production)
#if DEBUG
        services.AddDebugTextExtractor(o =>
        {
            o.Result = "Extra Training";
            // o.Result = "Sunny Day Standoff";
            o.Confidence = 1f;
        });
#else
        services.AddSingleton<ITextExtractor, TesseractTextExtractor>();
#endif

        return services;
    }
}