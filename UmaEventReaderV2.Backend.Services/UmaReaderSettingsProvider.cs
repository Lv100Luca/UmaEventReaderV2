using System.Drawing;

namespace UmaEventReaderV2.Services;

public class UmaReaderSettingsProvider
{
    public TimeSpan ScanInterval { get; set; } = TimeSpan.FromSeconds(250);

    public bool TryDetermineCharacter { get; set; } = false;
    public string CareerCharacterOverride { get; set; } = string.Empty;

    // TODO(LDI): replace this with selector for resolutions?
    public bool OverrideArea { get; set; } = false;
    public Rectangle? AreaOverride { get; set; }
}