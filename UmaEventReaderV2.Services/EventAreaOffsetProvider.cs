using System.Drawing;

namespace UmaEventReaderV2.Services;

public class EventAreaOffsetProvider(EventAreaOffsetOptions options)
{
    public required int Offset { get; init; }

    public Rectangle OffsetRectangle(Rectangle eventArea)
    {
        return eventArea with { X = eventArea.X + Offset, Width = eventArea.Width - Offset };
    }
}

public class EventAreaOffsetOptions
{
    public int Offset { get; set; }
}