using System.Drawing;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Services;

public class UmaReaderSettingsProvider
{
    public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMilliseconds(250);

    public bool TryDetermineCharacter { get; set; } = false;
    public Uma? CareerCharacterOverride { get; set; }

    // TODO(LDI): replace this with selector for resolutions?
    public bool OverrideArea { get; set; } = false;
    public Rectangle? AreaOverride { get; set; }
}