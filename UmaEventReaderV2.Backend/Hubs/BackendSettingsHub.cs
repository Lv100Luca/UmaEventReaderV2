using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models.dto;
using UmaEventReaderV2.Services;
using UmaEventReaderVs.WinForms;

namespace UmaEventReaderV2.Backend.Hubs;

// TODO(LDI): Create BackendSettingsService to manage higher level settings features
public class BackendSettingsHub
(
    UmaReaderSettingsProvider settings,
    IScreenshotAreaProvider screenshotAreaProvider,
    ScreenshotAreaSelector screenshotAreaSelector,
    ILogger<BackendEventHub> logger) : Hub
{
    public async Task<SettingsDto> GetSettings()
    {
        return new SettingsDto
        {
            ScanInterval = settings.ScanInterval,
            CareerCharacterOverride = settings.CareerCharacterOverride,
            TryDetermineCharacter = settings.TryDetermineCharacter,
        };
    }

    public async Task SaveSettings(SettingsDto dto)
    {
        settings.TryDetermineCharacter = dto.TryDetermineCharacter;
        settings.CareerCharacterOverride = dto.CareerCharacterOverride;
        settings.ScanInterval = dto.ScanInterval;
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