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

public partial class UmaEventReader(
    IUmaEventService eventService,
    IScreenshotAreaProvider screenshotAreaProvider,
    IScreenshotProvider screenshotProvider,
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

        var debugSaveImage = false;

        if (debugSaveImage)
            DebugSaveImage(result);

        if (!ValidateText(result))
            return false;

        if (result.Text == previousText)
            return true;

        if (!retry)
            previousText = result.Text;

        await Console.Out.WriteLineAsync(result.Text);

        var events = RunSearch(result.Text);

        if (events.Count == 1)
        // foreach (var @event in events)
        // {
            SaveImage(events.First().EventName, result.Metadata.ProcessedImage, result.Metadata.RawImage);
        // }

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
        var processed = ImagePreProcessor.Process(rawScreenshot, skipBorder: true);

        var result = await textExtractor.ExtractTextAsync(processed);

        result.Metadata.RawImage = rawScreenshot;

        return result;
    }

    private static bool ValidateText(TextExtractorResult result)
    {
        return result.Metadata.MeanConfidence > ConfidenceThreshold && result.Text.Length >= 3 && !string.IsNullOrWhiteSpace(result.Text);
    }

    // todo to service?
    public static string GetSolutionCapturesPath()
    {
        // Go up 3-4 levels from bin/Debug/netX.0/ to solution root
        var solutionRoot = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, @"..\..\..\..")
        );

        return Path.Combine(solutionRoot, "captures");
    }

    private static Guid CreateGuidFromString(string input)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }

    private static void SaveImage(string eventName, Bitmap? processed, Bitmap? raw)
    {
        var baseDir = GetSolutionCapturesPath();
        var eventId = CreateGuidFromString(eventName);
        var eventDir = Path.Combine(baseDir, eventId.ToString());

        Directory.CreateDirectory(eventDir);

        // write eventName.txt only if it doesn’t exist
        var namePath = Path.Combine(eventDir, "eventName.txt");
        if (!File.Exists(namePath))
        {
            File.WriteAllText(namePath, eventName);
        }

        // save images with unique filenames
        SaveWithIncrement(raw, eventDir, "raw");
        SaveWithIncrement(processed, eventDir, "processed");
    }

    private static void SaveWithIncrement(Bitmap? bmp, string dir, string prefix)
    {
        if (bmp == null) return;

        var filePath = Path.Combine(dir, $"{prefix}.png");
        int counter = 1;

        // if file exists, increment until free name is found
        while (File.Exists(filePath))
        {
            filePath = Path.Combine(dir, $"{prefix}_{counter}.png");
            counter++;
        }

        bmp.Save(filePath, ImageFormat.Png);
    }

    private static void DebugSaveImage(TextExtractorResult result, string manEventName = "")
    {
        var baseDir = "captures";
        var debugPath = Path.Combine(baseDir, "debug");

        if (!string.IsNullOrWhiteSpace(manEventName))
        {
            debugPath = Path.Combine(debugPath, manEventName);

            var namePath = Path.Combine(debugPath, "eventName.txt");
            if (!File.Exists(namePath))
            {
                File.WriteAllText(namePath, debugPath);
            }
        }

        Directory.CreateDirectory(baseDir);
        Directory.CreateDirectory(debugPath);

        SaveWithIncrement(result.Metadata.RawImage, debugPath, "raw");
        SaveWithIncrement(result.Metadata.ProcessedImage, debugPath, "processed");
    }

    public static void DebugSaveImage(Bitmap bmp)
    {
        var baseDir = GetSolutionCapturesPath();
        var debugPath = Path.Combine(baseDir, "debug\\local");

        Directory.CreateDirectory(baseDir);
        Directory.CreateDirectory(debugPath);

        var path = Path.Combine(debugPath, "debug.png");

        bmp.Save(path, ImageFormat.Png);
        // SaveWithIncrement(bmp, debugPath, "debug");
    }
}