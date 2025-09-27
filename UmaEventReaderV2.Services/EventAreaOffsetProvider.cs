using System.Drawing;
using Microsoft.Extensions.Options;

namespace UmaEventReaderV2.Services;

public class EventAreaOffsetProvider(IOptions<EventAreaOffsetOptions> options)
{
    public required int Offset { get; init; } = options.Value.Offset;

    public Rectangle OffsetRectangle(Rectangle eventArea)
    {
        return eventArea with { X = eventArea.X + Offset, Width = eventArea.Width - Offset };
    }
}

public class EventAreaOffsetOptions
{
    public int Offset { get; set; }
}