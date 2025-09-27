using System.Drawing;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public class StaticScreenshotAreaProvider(EventAreaOffsetProvider offsetProvider) : IScreenshotAreaProvider
{
    // todo make this configurable/ be able to pass this in
    private int Offset => offsetProvider.Offset;

    private const int X = 322;
    private const int Y = 269;
    private const int Width = 411;
    private const int Height = 49;

    public ScreenshotArea GetEventArea()
    {
        var rect = new Rectangle(X, Y, Width, Height);

        return new ScreenshotArea
        {
            Name = "Full Event Area",
            Area = rect
        };
    }

    public ScreenshotArea GetOffsetEventArea()
    {
        var rect = new Rectangle(X + Offset, Y, Width - Offset, Height);

        return new ScreenshotArea
        {
            Name = "Offset Event Area",
            Area = rect
        };
    }
}