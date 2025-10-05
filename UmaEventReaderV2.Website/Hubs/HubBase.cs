using Microsoft.AspNetCore.SignalR.Client;

namespace UmaEventReaderV2.Website.Hubs;

public abstract class HubBase(string path)
{
    private const string BaseUrl = "https://localhost:7252";

    public readonly HubConnection Connection = CreateHubConnection(path);

    private static HubConnection CreateHubConnection(string path)
    {
        return new HubConnectionBuilder()
            .WithUrl($"{BaseUrl}/{path}")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartHubIfDisconnected()
    {
        if (Connection.State == HubConnectionState.Disconnected)
            await Connection.StartAsync();
    }

    protected async Task<TResult> InvokeAsync<TRequest, TResult>(string methodName, TRequest request)
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<TResult>(methodName, request);
    }
}