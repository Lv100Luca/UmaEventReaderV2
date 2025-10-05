using Microsoft.AspNetCore.SignalR;

namespace UmaEventReaderV2.Backend.Hubs;

public class BackendConnectionStatusHub : Hub
{
    public async Task<bool> Ping()
    {
        return true;
    }
}