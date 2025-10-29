using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Services;
using UmaEventReaderV2.Services.Emitters;
using UmaEventReaderV2.Services.Mapper;
using UmaEventReaderV2.Services.Repositories;
using UmaEventReaderVs.WinForms;

namespace UmaEventReaderV2.Backend.Extensions;

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
            .AddSingleton<ISettingsService,SettingsService>()
            .AddSingleton<SelectAreaOverlay>()
            .AddEventAreaOffsetProvider(options => options.Offset = 55)
            // Alternative provider (for offline testing)
            .AddSingleton<IUmaEventJsonProvider, PlaywrightUmaEventJsonProvider>()
            // .AddSingleton<IUmaEventJsonProvider, StaticUmaEventJsonProvider>()
            // use selector
            .AddSingleton<ScreenshotAreaSelector>()
            .AddSingleton<IScreenshotAreaProvider, ScreenshotAreaProvider>()
            .AddSingleton<IScreenshotProvider, ScreenshotProvider>();

        // Repositories
        services
            .AddSingleton<IUmaRepository, UmaRepository>()
            .AddSingleton<IUmaEventRepository, UmaEventMemoryRepository>()
            .AddSingleton<IUmaSkillRepository, UmaSkillRepository>()
            .AddSingleton<IRepositoryInitializer, RepositoryInitializer>();

        // Core Services
        services
            .AddSingleton<UmaEventMapperV2>()
            .AddSingleton<IEventEmitter, CachingEventEmitter>()
            // .AddSingleton<IEventEmitter, CachingEventEmitter>()
            .AddSingleton<OcrService>()
            .AddHostedService<UmaEventReader>();

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