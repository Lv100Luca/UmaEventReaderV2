using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderVs.WinForms;

namespace UmaEventReaderV2.Backend.Hubs;

public class BackendSettingsHub
(
    ISettingsService settingsService,
    IScreenshotAreaProvider screenshotAreaProvider,
    ScreenshotAreaSelector screenshotAreaSelector,
    ILogger<BackendEventHub> logger) : Hub
{
    public async Task<UmaEventReaderSettings> GetSettings()
    {
        return settingsService.Settings;
    }

    public async Task SaveSettings(UmaEventReaderSettings settings)
    {
        Action<UmaEventReaderSettings> update = readerSettings =>
        {
            readerSettings.ScanInterval = settings.ScanInterval;
            readerSettings.FilteredCharacter = settings.FilteredCharacter;
            readerSettings.EventArea = settings.EventArea;
        };

        await settingsService.UpdateSettingsAsync(update);
    }

    public async Task<bool> SelectNewAreaAsync()
    {
        Rectangle newArea;

        try
        {
            newArea = screenshotAreaSelector.SelectArea();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error selecting area");;

            return false;
        }

        screenshotAreaProvider.UpdateBaseArea(newArea);

        return true;
    }

    public async Task<bool> SetNewAreaAsync(Rectangle area)
    {
        screenshotAreaProvider.UpdateBaseArea(area);

        return true;
    }
}