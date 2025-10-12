using System.Drawing;

namespace UmaEventReaderV2.Common.Models;

public class UmaEventReaderSettings
{
    public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMilliseconds(250);

    public Uma? FilteredCharacter { get; set; }

    public Rectangle? EventArea { get; set; }
}