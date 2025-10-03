using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models.dto;

namespace UmaEventReaderV2.Website.Hubs;

public class WebSettingsHub() : HubBase("/settings")
{
    public async Task<SettingsDto> GetSettingsAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<SettingsDto>("GetSettings");
    }

    public async Task SaveSettingsAsync(SettingsDto dto)
    {
        await StartHubIfDisconnected();

        await Connection.InvokeAsync("SaveSettings", dto);
    }
}