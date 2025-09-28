using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Models;

public class EventBatch
{
    public IEnumerable<UmaEvent> Events { get; set; } = new List<UmaEvent>();

    public static EventBatch From(IEnumerable<UmaEvent> events)
    {
        return new EventBatch
        {
            Events = events
        };
    }
}