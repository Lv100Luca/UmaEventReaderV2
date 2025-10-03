using Microsoft.AspNetCore.SignalR;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Common.Models;
using UmaEventReaderV2.Common.Models.Enums;
using UmaEventReaderV2.Services;

namespace UmaEventReaderV2.Web.Hubs;

public class EventHub(IUmaEventRepository eventRepository, UmaEventReader reader) : Hub
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

    public async Task RestartReaderAsync()
    {
        await reader.RunAsync();

        await Console.Out.WriteLineAsync("Restarted reader");
    }
}