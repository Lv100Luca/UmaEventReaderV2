using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class ScreenshotProvider(ILogger<ScreenshotProvider> logger) : IScreenshotProvider
{
    public Bitmap? TakeScreenshot(Rectangle area)
    {
        if (area.Width <= 0 || area.Height <= 0)
            return null;

        var bounds = ScreenBounds.VirtualScreen;

        if (!bounds.Contains(area))
        {
            area.Intersect(bounds);

            if (area.Width <= 0 || area.Height <= 0)
                return null;
        }

        var bmp = new Bitmap(area.Width, area.Height);
        var g = Graphics.FromImage(bmp);
        g.CopyFromScreen(area.Location, Point.Empty, area.Size);

        return bmp;
    }
}

internal static partial class ScreenBounds
{
    private const int SM_XVIRTUALSCREEN = 76;
    private const int SM_YVIRTUALSCREEN = 77;
    private const int SM_CXVIRTUALSCREEN = 78;
    private const int SM_CYVIRTUALSCREEN = 79;

    [LibraryImport("user32.dll")]
    private static partial int GetSystemMetrics(int nIndex);

    public static Rectangle VirtualScreen
    {
        get
        {
            int x = GetSystemMetrics(SM_XVIRTUALSCREEN);
            int y = GetSystemMetrics(SM_YVIRTUALSCREEN);
            int width = GetSystemMetrics(SM_CXVIRTUALSCREEN);
            int height = GetSystemMetrics(SM_CYVIRTUALSCREEN);

            return new Rectangle(x, y, width, height);
        }
    }
}