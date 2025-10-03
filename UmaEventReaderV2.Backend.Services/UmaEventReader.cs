using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public class UmaEventReader(
    IScreenshotAreaProvider screenshotAreaProvider,
    OcrService ocrService,
    IUmaEventService eventService,
    IEventEmitter eventEmitter,
    ILogger<UmaEventReader> logger,
    float confidenceThreshold = 0.6f)
{
    private readonly TimeSpan checkInterval = TimeSpan.FromMilliseconds(250);

    private string lastText = string.Empty;
    private List<string> lastEventIds = [];

    // public event Action<string>? OnLog;
    // public event Action<EventBatch>? OnEventFound;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await eventService.InitializeDataAsync();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(checkInterval, cancellationToken);

                var result = TryProcessAreas(screenshotAreaProvider.GetAllAreas(), out var events);

                if (result == null)
                    continue;

                await eventEmitter.EmitEventsAsync(events);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in UmaEventReader");

            await RunAsync(cancellationToken);

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

            var events = eventService.GetAllWhereNameIsLike(result.Text).ToList();

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