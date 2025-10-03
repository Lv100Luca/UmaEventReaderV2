using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models.dto;

namespace UmaEventReaderV2.Website.Hubs;

public class WebSettingsHub(HubConnection hubConnection)
{
    private async Task StartHubIfDisconnected()
    {
        if (hubConnection.State == HubConnectionState.Disconnected)
            await hubConnection.StartAsync();
    }

    private async Task<TResult> InvokeAsync<TRequest, TResult>(string methodName, TRequest request)
    {
        await StartHubIfDisconnected();

        return await hubConnection.InvokeAsync<TResult>(methodName, request);
    }

    public async Task<SettingsDto> GetSettingsAsync()
    {
        await StartHubIfDisconnected();

        return await hubConnection.InvokeAsync<SettingsDto>("GetSettings");
    }

    public async Task SaveSettingsAsync(SettingsDto dto)
    {
        await StartHubIfDisconnected();

        await hubConnection.InvokeAsync("SaveSettings", dto);
    }
}