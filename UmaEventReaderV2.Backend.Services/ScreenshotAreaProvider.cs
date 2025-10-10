using System.Drawing;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public class ScreenshotAreaProvider(EventAreaOffsetProvider offsetProvider) : IScreenshotAreaProvider
{
    // TODO(LDI): Load the base from settings
    private Rectangle Area { get; set; } = GetBase();

    private ScreenshotArea GetMainArea()
    {
        return new ScreenshotArea
        {
            Name = "Main",
            Area = Area
        };
    }

    private ScreenshotArea GetOffsetArea()
    {
        return new ScreenshotArea
        {
            Name = "Offset",
            Area = offsetProvider.OffsetRectangle(Area)
        };
    }

    public void UpdateBaseArea(Rectangle area)
    {
        Area = area;
    }

    public IEnumerable<ScreenshotArea> GetAllAreas()
    {
        yield return GetMainArea();
        yield return GetOffsetArea();
    }

    private static Rectangle GetBase()
    {
        const int x = 322;
        const int y = 269;
        const int width = 411;
        const int height = 49;

        return new Rectangle(x, y, width, height);
    }
}