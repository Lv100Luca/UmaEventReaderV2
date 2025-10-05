using Microsoft.AspNetCore.SignalR.Client;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Website.Hubs;

public class UmaEventHub : HubBase
{
    public event Action<EventBatch>? OnEventFound;

    public UmaEventHub() : base("events")
    {
        Connection.On<EventBatch>("OnEventFound", batch => OnEventFound?.Invoke(batch));
    }

    public async Task<IEnumerable<UmaEvent>> InvokeSearchAsync(EventSearchModel searchModel)
    {
        return await InvokeAsync<EventSearchModel, IEnumerable<UmaEvent>>("Search", searchModel);
    }
}