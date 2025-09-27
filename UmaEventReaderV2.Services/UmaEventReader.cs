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
    private readonly TimeSpan checkInterval = TimeSpan.FromSeconds(1);
    private string previousText = string.Empty;

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

                if (result.Text != previousText)
                {
                    previousText = result.Text;
                    OnLog?.Invoke(result.ToString());
                    OnLog?.Invoke($"Emitting {events.Count} events");
                }

                EmitEvents(events);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    private void EmitEvents(IEnumerable<UmaEventEntity> e)
    {
        var batch = new EventBatch
        {
            Events = e
        };

        OnEventFound?.Invoke(batch);
    }

    private TextExtractorResult? TryProcessAreas(IEnumerable<ScreenshotArea> areas, out List<UmaEventEntity> foundEvents)
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

            OnLog?.Invoke(log);

            if (events.Count != 0)
            {
                foundEvents = events;

                return result;
            }
        }


        return null; // no area yielded any events
    }
}