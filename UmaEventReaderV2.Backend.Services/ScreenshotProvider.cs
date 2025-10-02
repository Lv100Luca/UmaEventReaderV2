using System.Drawing;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class ScreenshotProvider : IScreenshotProvider
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
        catch (Exception ex)
        {
            // Log but don't kill the app
            Console.WriteLine($"[ScreenshotProvider] Failed to capture {area}: {ex.Message}");
            return null;
        }
    }
}