using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Web.Hubs;

public class EventHub(IUmaEventService eventService) : Hub
{
    // called by backend when a new event is found
    public async Task BroadcastEvent(UmaEvent umaEvent)
    {
        await Clients.All.SendAsync("OnEventFound", umaEvent);
    }

    public async Task BroadcastLog(string message)
    {
        await Clients.All.SendAsync("OnLog", message);
    }

    public async Task<IEnumerable<UmaEvent>> Search(EventSearchModel request)
    {
        return eventService.GetAllWhereNameIsLike(request.Name);
    }
}