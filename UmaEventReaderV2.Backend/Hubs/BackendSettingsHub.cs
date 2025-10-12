using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderVs.WinForms;

namespace UmaEventReaderV2.Backend.Hubs;

public class BackendSettingsHub(
    ISettingsService settingsService,
    IScreenshotProvider screenshotProvider,
    ScreenshotAreaSelector screenshotAreaSelector,
    ILogger<BackendEventHub> logger)
    : Hub, ISettingsHub
{
    public async Task<UmaEventReaderSettings> GetSettingsAsync()
    {
        return settingsService.Settings;
    }

    public async Task SaveSettingsAsync(UmaEventReaderSettings settings)
    {
        await settingsService.UpdateSettingsAsync(readerSettings =>
        {
            readerSettings.ScanInterval = settings.ScanInterval;
            readerSettings.FilteredCharacter = settings.FilteredCharacter;
            readerSettings.EventArea = settings.EventArea;
        });
    }

    public async Task<Rectangle?> SelectNewAreaAsync()
    {
        try
        {
            var newArea = screenshotAreaSelector.SelectArea();

            logger.LogInformation("Selected Area {@Area}", newArea);

            return newArea;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error selecting area");

            return null;
        }
    }

    public async Task<string?> CapturePreviewScreenshotAsync(Rectangle area)
    {
        var screenshot = screenshotProvider.TakeScreenshot(area);

        if (screenshot is null)
        {
            logger.LogWarning("No screenshot taken");

            return null;
        }

        using var ms = new MemoryStream();
        screenshot.Save(ms, ImageFormat.Png);
        return Convert.ToBase64String(ms.ToArray());
    }
}