using Microsoft.AspNetCore.SignalR;

namespace UmaEventReaderV2.Web.Hubs;

public class BackendConnectionStatusHub : Hub
{
    public async Task<bool> Ping()
    {
        return true;
    }
}