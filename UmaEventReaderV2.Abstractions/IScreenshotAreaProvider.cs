using System.Drawing;

namespace UmaEventReaderV2.Abstractions;

public interface IScreenshotAreaProvider
{
    /// <summary>
    /// Returns the Area of a career event
    /// </summary>
    Rectangle GetEventArea();

    /// <summary>
    /// Returns a slightly offset area of the career event.
    /// Area is shifted slightly to the right.
    /// Used for when the current event is prepended with a trainee icon
    /// </summary>
    Rectangle GetFallbackEventArea();
}