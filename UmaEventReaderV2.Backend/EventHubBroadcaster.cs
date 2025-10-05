using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Backend.Hubs;

namespace UmaEventReaderV2.Backend;

public class EventHubBroadcaster
{
    private readonly IHubContext<BackendEventHub> hub;
    private readonly IEventEmitter eventEmitter;

    public EventHubBroadcaster(IHubContext<BackendEventHub> hub, IEventEmitter eventEmitter)
    {
        this.hub = hub;
        this.eventEmitter = eventEmitter;

        this.eventEmitter.OnEventFound += async e =>
            await hub.Clients.All.SendAsync("OnEventFound", e);
    }
}