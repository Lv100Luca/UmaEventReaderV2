using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Services;
using UmaEventReaderV2.Web.Hubs;

namespace UmaEventReaderV2.Web;

public class EventHubBroadcaster
{
    private readonly IHubContext<EventHub> hub;

    public EventHubBroadcaster(UmaEventReader reader, IHubContext<EventHub> hub)
    {
        this.hub = hub;

        reader.OnEventFound += async e =>
            await hub.Clients.All.SendAsync("OnEventFound", e);

        reader.OnLog += async msg =>
            await hub.Clients.All.SendAsync("OnLog", msg);
    }
}