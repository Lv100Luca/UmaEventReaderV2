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
    public event Action<TextExtractorResult>? OnResult;
    public event Action<UmaEventEntity>? OnEventFound;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(checkInterval, cancellationToken);

            var result = await TryProcessAreasAsync(screenshotAreaProvider.GetAllAreas());
            if (result == null)
                continue;

            if (result.Text == previousText)
                continue;

            previousText = result.Text;

            OnLog?.Invoke($"Result: '{result.Text}' ({result.Metadata.MeanConfidence:0.00})");
            OnResult?.Invoke(result);

            var events = eventService.GetAllWhereNameIsLike(result.Text);
            foreach (var e in events)
                OnEventFound?.Invoke(e);
        }
    }

    private async Task<TextExtractorResult?> TryProcessAreasAsync(IEnumerable<Rectangle> areas)
    {
        foreach (var area in areas)
        {
            var result = await ocrService.ExtractText(area);

            if (TextValidator.IsValid(result, confidenceThreshold))
                return result;
        }

        return null;
    }
}
