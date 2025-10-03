using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Common.Models.dto;
using UmaEventReaderV2.Services;

namespace UmaEventReaderV2.Web.Hubs;

public class BackendSettingsHub(UmaReaderSettingsProvider settings) : Hub
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
}