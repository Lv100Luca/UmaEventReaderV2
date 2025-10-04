using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public class UmaEventReader(
    IScreenshotAreaProvider screenshotAreaProvider,
    OcrService ocrService,
    IUmaEventRepository umaEventRepository,
    IRepositoryInitializer initializer,
    IEventEmitter eventEmitter,
    ILogger<UmaEventReader> logger,
    UmaReaderSettingsProvider settings,
    float confidenceThreshold = 0.6f) : BackgroundService
{
    private string lastText = string.Empty;
    private List<string> lastEventIds = [];

    override public async Task StartAsync(CancellationToken cancellationToken)
    {
        await initializer.InitializeAsync(cancellationToken);


        await base.StartAsync(cancellationToken);
    }

    override public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping UmaEventReader");
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(settings.ScanInterval, stoppingToken);

                var result = TryProcessAreas(screenshotAreaProvider.GetAllAreas(), out var events);

                if (result == null)
                    continue;

                await eventEmitter.EmitEventsAsync(events);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in UmaEventReader");

            await ExecuteAsync(stoppingToken);

            logger.LogInformation("Restarted Service");
        }
    }

    private TextExtractorResult? TryProcessAreas(IEnumerable<ScreenshotArea> areas, out List<UmaEvent> foundEvents)
    {
        foundEvents = [];

        foreach (var area in areas)
        {
            var result = ocrService.ExtractText(area);

            if (!TextValidator.IsValid(result, confidenceThreshold))
                continue;

            var events = umaEventRepository.GetAllForCharacterWhereNameIsLike(settings.CareerCharacterOverride, result.Text)
                .ToList();

            // var log = new StringBuilder()
            //     .AppendLine($"Processed {area.Name}")
            //     .AppendLine($"- '{result.Text}'")
            //     .AppendLine($"- {events.Count} events found")
            //     .ToString();

            // Console.Out.WriteLine(log);

            if (events.Count > 0)
            {
                foundEvents = events;

                return result;
            }
        }

        return null; // no area yielded any events
    }
}