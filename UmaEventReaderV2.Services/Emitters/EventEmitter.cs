using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

public class EventEmitter : IEventEmitter
{
    public event Action<EventBatch>? OnEventFound;

    public async Task EmitEventsAsync(IEnumerable<UmaEvent> events)
    {
        OnEventFound?.Invoke(EventBatch.From(events));
    }
}