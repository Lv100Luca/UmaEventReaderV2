using System.Drawing;
using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

public class DebugScreenshotProvider :IScreenshotProvider
{
    public Bitmap TakeScreenshot(Rectangle area)
    {
        var basedir = UmaEventReader.GetSolutionCapturesPath();
        var debugDir = Path.Combine(basedir, "debug");

        var filename = "raw_1.png";

        var images = Directory.GetFiles(debugDir, "*.png");
        var image = images.First(i => GetFilename(i) == filename);

        return new Bitmap(image);
    }

    private string GetFilename(string path)
    {
        var parts = path.Split('\\');

        return parts.Last();
    }
}