using System.Drawing;
using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Website.Hubs;

public class WebSettingsHub() : HubBase("settings")
{
    public async Task<UmaEventReaderSettings> GetSettingsAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<UmaEventReaderSettings>("GetSettings");
    }

    public async Task SaveSettingsAsync(UmaEventReaderSettings settings)
    {
        await StartHubIfDisconnected();

        await Connection.InvokeAsync("SaveSettings", settings);
    }

    public async Task<bool> SelectNewAreaAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<bool>("SelectNewAreaAsync");
    }

    public async Task<bool> SetNewAreaAsync(Rectangle area)
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<bool>("SetNewAreaAsync", area);
    }
}