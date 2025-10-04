using System.Drawing;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class ScreenshotProvider(ILogger<ScreenshotProvider> logger) : IScreenshotProvider
{
    public Bitmap? TakeScreenshot(Rectangle area)
    {
        if (area.Width <= 0 || area.Height <= 0)
            return null;

        try
        {
            var bmp = new Bitmap(area.Width, area.Height);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(area.Location, Point.Empty, area.Size);

            return bmp;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Failed to take screenshot");

            return null;
        }
    }
}