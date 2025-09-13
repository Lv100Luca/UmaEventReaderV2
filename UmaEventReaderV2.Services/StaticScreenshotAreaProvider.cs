using System.Drawing;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class StaticScreenshotAreaProvider : IScreenshotAreaProvider
{
    // todo make this configurable/ be able to pass this in
    private const int Offset = 60;

    private const int X = 322;
    private const int Y = 269;
    private const int Width = 411;
    private const int Height = 49;

    public Rectangle GetEventArea()
    {
        return new Rectangle(X, Y, Width, Height);
    }

    public Rectangle GetFallbackEventArea()
    {
        return new Rectangle(X + Offset, Y, Width - Offset, Height);
    }
}