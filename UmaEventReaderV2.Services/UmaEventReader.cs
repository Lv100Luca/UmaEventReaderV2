using System.Drawing;
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

    private List<UmaEventEntity> events = [];

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(checkInterval, cancellationToken);

                var result = TryProcessAreas(screenshotAreaProvider.GetAllAreas());

                if (result == null)
                    continue;

                if (result.Text != previousText)
                {
                    previousText = result.Text;

                    OnLog?.Invoke(result.ToString());

                    events = eventService.GetAllWhereNameIsLike(result.Text).ToList();

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

    private TextExtractorResult? TryProcessAreas(IEnumerable<Rectangle> areas)
    {
        foreach (var area in areas)
        {
            var result = ocrService.ExtractText(area);

            if (TextValidator.IsValid(result, confidenceThreshold))
                return result;
        }

        return null;
    }
}