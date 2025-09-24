using System.Drawing;
using System.Drawing.Imaging;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

public class DebugImageSaver
{
    public const string CaptureFolder = "captures";
    public const string EventNameFileName = "eventNames.txt";
    public const string RawPrefix = "raw";
    public const string ProcessedPrefix = "processed";

    public static void SaveImage(UmaEventEntity umaEvent, Bitmap? processed, Bitmap? raw)
    {
        var eventHash = umaEvent.GetHashCode();
        var eventDir = Path.Combine(CaptureFolder, eventHash.ToString());

        Directory.CreateDirectory(eventDir);

        // write eventName.txt only if it doesn’t exist
        var namePath = Path.Combine(eventDir, EventNameFileName);

        if (!File.Exists(namePath))
        {
            File.WriteAllText(namePath, umaEvent.EventName);
        }

        // save images with unique filenames
        SaveIfNotExists(raw, eventDir, RawPrefix);
        SaveIfNotExists(processed, eventDir, ProcessedPrefix);
    }


    private static void SaveIfNotExists(Bitmap? bmp, string dir, string prefix)
    {
        if (bmp == null) return;

        var filePath = Path.Combine(dir, $"{prefix}.png");

        if (!File.Exists(filePath))
        {
            bmp.Save(filePath, ImageFormat.Png);
        }
    }
}
