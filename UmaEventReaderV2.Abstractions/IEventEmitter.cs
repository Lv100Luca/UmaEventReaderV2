using UmaEventReaderV2.Models;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Abstractions;

public interface IEventEmitter
{
    public event Action<EventBatch> OnEventFound;

    Task EmitEventsAsync(IEnumerable<UmaEvent> events);
}