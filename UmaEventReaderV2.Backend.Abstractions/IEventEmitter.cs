using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Abstractions;

public interface IEventEmitter
{
    public event Action<EventBatch> OnEventFound;

    Task EmitEventsAsync(IEnumerable<UmaEvent> events);
}