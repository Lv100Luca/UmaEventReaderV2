using System.Drawing;

namespace UmaEventReaderV2.Services;

public class EventAreaOffsetProvider(int offset)
{
    public Rectangle OffsetRectangle(Rectangle eventArea)
    {
        return eventArea with { X = eventArea.X + offset, Width = eventArea.Width - offset };
    }
}