using System.Drawing;

namespace UmaEventReaderV2.Models;

public class ScreenshotArea
{
    public required string Name { get; set; } = string.Empty;
    public required Rectangle Area { get; set; }
}