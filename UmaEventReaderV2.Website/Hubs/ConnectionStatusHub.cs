using Microsoft.AspNetCore.SignalR.Client;

namespace UmaEventReaderV2.Website.Hubs;

public class ConnectionStatusHub() : HubBase("status")
{
    public async Task<bool> PingAsync()
    {
        await StartHubIfDisconnected();

        return await Connection.InvokeAsync<bool>("Ping");
    }
}