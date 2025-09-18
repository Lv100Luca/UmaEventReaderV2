using System.Drawing;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class ScreenshotProvider : IScreenshotProvider
{
    public Bitmap TakeScreenshot(Rectangle area)
    {
        if (area.Width <= 0 || area.Height <= 0)
            throw new ArgumentException("Rectangle width and height must be positive");

        var bmp = new Bitmap(area.Width, area.Height);
        using var g = Graphics.FromImage(bmp);

        g.CopyFromScreen(area.Location, Point.Empty, area.Size);

        return bmp;
    }
}