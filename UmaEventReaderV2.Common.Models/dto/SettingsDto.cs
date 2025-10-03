namespace UmaEventReaderV2.Common.Models.dto;

public class SettingsDto
{
    public TimeSpan ScanInterval { get; set; } = TimeSpan.FromMilliseconds(250);

    public bool TryDetermineCharacter { get; set; } = false;
    public Uma? CareerCharacterOverride { get; set; }

    // TODO(LDI): replace this with selector for resolutions?
    // public bool OverrideArea { get; set; } = false;
    // public Rectangle? AreaOverride { get; set; }
}