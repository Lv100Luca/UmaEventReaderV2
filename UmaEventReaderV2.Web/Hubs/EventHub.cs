using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Web.Hubs;

public class EventHub : Hub
{
    // called by backend when a new event is found
    public async Task BroadcastEvent(UmaEventEntity umaEvent)
    {
        await Clients.All.SendAsync("OnEventFound", umaEvent);
    }

    public async Task BroadcastLog(string message)
    {
        await Clients.All.SendAsync("OnLog", message);
    }
}
