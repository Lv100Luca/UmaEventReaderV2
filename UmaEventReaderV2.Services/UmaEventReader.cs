using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services.Utility;

namespace UmaEventReaderV2.Services;

public partial class UmaEventReader(
    IUmaEventService eventService,
    IScreenshotAreaProvider screenshotAreaProvider,
    ScreenshotProvider screenshotProvider,
    ITextExtractor textExtractor
)
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
        var result = await CaptureAndExtractScreenshotText(area);

        if (!ValidateText(result))
            return false;

        var cleanedText = Clean(result.Text);

        if (cleanedText == previousText)
            return true;

        if (!retry)
            previousText = cleanedText;

        var events = RunSearch(cleanedText);

        if (events.Count == 1 && result.Metadata.ProcessedImage is {} bmp)
            SaveImage(bmp, events.First().EventName);

        return events.Count > 0;
    }

    private List<UmaEventEntity> RunSearch(string eventName)
    {
        var events = eventService.GetAllWhereNameIsLike(eventName).ToList();

        foreach (var e in events)
            Console.Out.WriteLine(e);

        return events;
    }

    private async Task<TextExtractorResult> CaptureAndExtractScreenshotText(Rectangle area)
    {
        var rawScreenshot = screenshotProvider.TakeScreenshot(area);
        var processed = ImagePreProcessor.AddBorder(ImagePreProcessor.IsolateText(rawScreenshot), 5, Color.Black);

        var result = await textExtractor.ExtractTextAsync(processed);

        result.Metadata.RawImage = rawScreenshot;

        return result;
    }

    private static bool ValidateText(TextExtractorResult result)
    {
        return result.Metadata.MeanConfidence > ConfidenceThreshold && result.Text.Length >= 3 && !string.IsNullOrWhiteSpace(result.Text);
    }

    private static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Replace("\r\n", "").Replace("\r", "").Trim();
        input = NonAlphaNum().Replace(input, ""); // trim non-alphanum edges
        input = NormalizeSpaces().Replace(input, " "); // normalize spaces

        return input;
    }

    private void SaveImage(Bitmap bmp, string filename)
    {
        var dir = "captures";
        // create dir
        Directory.CreateDirectory(dir);

        var path = $"dir\\{filename}.png";
        bmp.Save(path, ImageFormat.Png);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeSpaces();

    [GeneratedRegex(@"^[^\w\d]+|[^\w\d]+$")]
    private static partial Regex NonAlphaNum();
}