using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Abstractions.Repositories;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;

namespace UmaEventReaderV2.Backend.Hubs;

public class BackendEventHub(IUmaEventRepository eventRepository) : Hub
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
        return eventRepository.GetAllWhereNameIsLike(request.Name);
    }

    public async Task ReportStatus(BackendStatus status)
    {
        await Clients.All.SendAsync("OnStatusChanged", status.ToString());
    }
}