using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Website.Hubs;

public class UmaEventHub(HubConnection hubConnection)
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

    public async Task<IEnumerable<UmaEvent>> InvokeSearchAsync(EventSearchModel searchModel)
    {
        return await InvokeAsync<EventSearchModel, IEnumerable<UmaEvent>>("Search", searchModel);
    }
}