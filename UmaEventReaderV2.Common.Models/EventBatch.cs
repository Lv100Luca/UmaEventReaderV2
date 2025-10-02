namespace UmaEventReaderV2.Common.Models;

public class EventBatch
{
    public IEnumerable<UmaEvent> Events { get; set; } = new List<UmaEvent>();

    public static EventBatch From(IEnumerable<UmaEvent> events)
    {
        return new EventBatch
        {
            Events = events.ToList()
        };
    }

    override public bool Equals(object? obj) => Equals(obj as EventBatch);

    public bool Equals(EventBatch? other)
    {
        if (other is null)
            return false;

        var eventsA = Events.ToList();
        var eventsB = other.Events.ToList();

        if (eventsA.Count != eventsB.Count)
            return false;

        return eventsA.Select(e => e.Name)
            .SequenceEqual(eventsB.Select(e => e.Name));
    }

    override public int GetHashCode()
    {
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        return HashCode.Combine(Events);
    }
}