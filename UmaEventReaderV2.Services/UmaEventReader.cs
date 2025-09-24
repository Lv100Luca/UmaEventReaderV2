using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services.Utility;

namespace UmaEventReaderV2.Services;

public class UmaEventReader(
    IUmaEventService eventService,
    IScreenshotAreaProvider screenshotAreaProvider,
    IScreenshotProvider screenshotProvider,
    ITextExtractor textExtractor,
    IUmaFrontend frontend)
{
    private readonly TimeSpan checkInterval = TimeSpan.FromSeconds(1);
    private const float ConfidenceThreshold = 0.6f;

    private string previousText = string.Empty;

    public async Task RunAsync()
    {
        while (true)
        {
            await Task.Delay(checkInterval);

            //try and process a certain area of the screen to figure out the event
            // should it fial -> try the next are

            if (await TryProcessAreaAsync(screenshotAreaProvider.GetEventArea()))
                continue;

            if (await TryProcessAreaAsync(screenshotAreaProvider.GetFallbackEventArea(), retry: true))
                continue;

            // todo: maybe check the choices as well to determine the event (later)
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task<bool> TryProcessAreaAsync(Rectangle area, bool retry = false)
    {
        var frontendText = frontend.GetSearchQuery();

        var searchText = TextExtractorResult.With(frontendText);
        var captureText = await CaptureAndExtractScreenshotText(area);

        TextExtractorResult? result = null;

        if (captureText.Metadata.MeanConfidence < ConfidenceThreshold || captureText.Text.Length < 3)
            result = searchText;
        else
        {
            result = captureText;
            frontend.ResetSearchQuery();
        }

        if (!ValidateText(result))
            return false;

        if (result.Text == previousText)
            return true;

        if (!retry)
            previousText = result.Text;

        var sb = new StringBuilder();

        sb.AppendLine("Result")
            .AppendLine($" - '{result.Text}'")
            .AppendLine($" - {result.Metadata.MeanConfidence}");

        await frontend.LogAsync(sb.ToString());

        var events = await RunSearch(result.Text);

        if (events.Count == 1)
            DebugImageSaver.SaveImage(events.First(), result.Metadata.ProcessedImage, result.Metadata.RawImage);

        return events.Count > 0;
    }

    private async Task<List<UmaEventEntity>> RunSearch(string eventName)
    {
        var events = eventService.GetAllWhereNameIsLike(eventName).ToList();

        foreach (var e in events)
            await frontend.ShowEventAsync(e);

        return events;
    }

    private async Task<TextExtractorResult> CaptureAndExtractScreenshotText(Rectangle area)
    {
        var rawScreenshot = screenshotProvider.TakeScreenshot(area);
        var processed = ImagePreProcessor.Process(rawScreenshot, skipBorder: true);

        var result = await textExtractor.ExtractTextAsync(processed);

        result.Metadata.RawImage = rawScreenshot;

        return result;
    }

    private static bool ValidateText(TextExtractorResult result)
    {
        return result.Text.Length >= 3 &&
               !string.IsNullOrWhiteSpace(result.Text);
    }

    // todo to service?
}