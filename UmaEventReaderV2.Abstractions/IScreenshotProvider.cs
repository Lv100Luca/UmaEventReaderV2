using System.Drawing;

namespace UmaEventReaderV2.Abstractions;

public interface IScreenshotProvider
{
    public Bitmap TakeScreenshot(Rectangle area);
}