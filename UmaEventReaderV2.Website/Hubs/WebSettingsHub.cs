using System.Drawing;
using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Website.Hubs;

public class WebSettingsHub() : HubBase("settings"), ISettingsHub
{
    public async Task<UmaEventReaderSettings> GetSettingsAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<UmaEventReaderSettings>(nameof(GetSettingsAsync));
    }

    public async Task SaveSettingsAsync(UmaEventReaderSettings settings)
    {
        await StartHubIfDisconnected();

        await Connection.InvokeAsync(nameof(SaveSettingsAsync), settings);
    }

    public async Task<Rectangle?> SelectNewAreaAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<Rectangle?>(nameof(SelectNewAreaAsync));
    }
}