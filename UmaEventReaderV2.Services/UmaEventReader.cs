using System.Text;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

public class UmaEventReader(
    IScreenshotAreaProvider screenshotAreaProvider,
    OcrService ocrService,
    IUmaEventService eventService,
    float confidenceThreshold = 0.6f)
{
    private readonly TimeSpan checkInterval = TimeSpan.FromMilliseconds(100);

    private string lastText = string.Empty;
    private List<string> lastEventIds = [];

    public event Action<string>? OnLog;
    public event Action<EventBatch>? OnEventFound;

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

                if (result.Text != lastText)
                {
                    lastText = result.Text;
                    OnLog?.Invoke(result.ToString());
                    OnLog?.Invoke($"Detected text: '{lastText}', found {events.Count} events");
                }

                // TODO(LDI): emit repeat event only every x repetitions
                // var currentIds = events.Select(e => e.Name).OrderBy(x => x).ToList();
                // if (!currentIds.SequenceEqual(lastEventIds))
                // {
                    // lastEventIds = currentIds;
                    EmitEvents(events);
                // }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void EmitEvents(IEnumerable<UmaEvent> e)
    {
        var batch = new EventBatch
        {
            Events = e
        };

        OnEventFound?.Invoke(batch);
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

            var log = new StringBuilder()
                .AppendLine($"Processed {area.Name}")
                .AppendLine($"- '{result.Text}'")
                .AppendLine($"- {events.Count} events found")
                .ToString();

            Console.Out.WriteLine(log);

            if (events.Count > 0)
            {
                foundEvents = events;
                return result;
            }
        }

        return null; // no area yielded any events
    }
}
