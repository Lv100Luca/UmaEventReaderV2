using System.Drawing;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Abstractions;

public interface IScreenshotAreaProvider
{
    /// <summary>
    /// Returns the Area of a career event
    /// </summary>
    ScreenshotArea GetEventArea();

    /// <summary>
    /// Returns a slightly offset area of the career event.
    /// Area is shifted slightly to the right.
    /// Used for when the current event is prepended with a trainee icon
    /// </summary>
    ScreenshotArea GetOffsetEventArea();

    IEnumerable<ScreenshotArea> GetAllAreas() => [GetEventArea(), GetOffsetEventArea()];
}