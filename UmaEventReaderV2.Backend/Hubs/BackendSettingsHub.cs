using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderVs.WinForms;

namespace UmaEventReaderV2.Backend.Hubs;

public class BackendSettingsHub(
    ISettingsService settingsService,
    IScreenshotAreaProvider screenshotAreaProvider,
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
}