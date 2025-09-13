using System.Drawing;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class ScreenshotAreaSelector : IScreenshotAreaProvider
{
    public Rectangle GetEventArea()
    {
        throw new NotImplementedException();
    }

    public Rectangle GetFallbackEventArea()
    {
        throw new NotImplementedException();
    }
}