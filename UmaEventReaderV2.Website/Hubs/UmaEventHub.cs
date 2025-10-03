using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Website.Hubs;

public class UmaEventHub() : HubBase("events")
{
    public async Task<IEnumerable<UmaEvent>> InvokeSearchAsync(EventSearchModel searchModel)
    {
        return await InvokeAsync<EventSearchModel, IEnumerable<UmaEvent>>("Search", searchModel);
    }
}