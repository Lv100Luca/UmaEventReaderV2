using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Models;

public class EventBatch
{
    public IEnumerable<UmaEventEntity> Events { get; set; } = new List<UmaEventEntity>();

    public static EventBatch From(IEnumerable<UmaEventEntity> events)
    {
        return new EventBatch
        {
            Events = events
        };
    }
}