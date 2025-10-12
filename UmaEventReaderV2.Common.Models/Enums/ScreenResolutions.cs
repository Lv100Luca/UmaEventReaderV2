using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace UmaEventReaderV2.Common.Models.Enums;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum ScreenResolutions
{
    Unknown,

    // 1080p
    R1080p_16_9,
    R1080p_16_10,
    R1080p_21_9,
    R1080p_32_9,

    // 1440p - 2k
    R1440p_16_9,
    R1440p_16_10,
    R1440p_21_9,
    R1440p_32_9,

    //2160p - 4k
    R2160p_16_9,
    R2160p_16_10,
    R2160p_21_9,
    R2160p_32_9
}

public static class ScreenResolutionsExtensions
{
    public static string ToDisplayString(this ScreenResolutions preset) => preset switch
    {
        ScreenResolutions.R1080p_16_9 => "1080p (16:9)",
        ScreenResolutions.R1080p_16_10 => "1080p (16:10)",
        ScreenResolutions.R1080p_21_9 => "1080p (21:9)",
        ScreenResolutions.R1080p_32_9 => "1080p (32:9)",
        ScreenResolutions.R1440p_16_9 => "1440p (16:9)",
        ScreenResolutions.R1440p_16_10 => "1440p (16:10)",
        ScreenResolutions.R1440p_21_9 => "1440p (21:9)",
        ScreenResolutions.R1440p_32_9 => "1440p (32:9)",
        ScreenResolutions.R2160p_16_9 => "2160p (16:9)",
        ScreenResolutions.R2160p_16_10 => "2160p (16:10)",
        ScreenResolutions.R2160p_21_9 => "2160p (21:9)",
        ScreenResolutions.R2160p_32_9 => "2160p (32:9)",
        _ => "Unknown"
    };
}

public static class ScreenResolutionsLookup
{
    private readonly static Dictionary<ScreenResolutions, Rectangle> PresetAreas = new()
    {
        { ScreenResolutions.R1080p_16_9, new Rectangle(240, 194, 550, 244) },
        { ScreenResolutions.R1080p_16_10, new Rectangle(0, 0, 1920, 1200) },
        { ScreenResolutions.R1080p_21_9, new Rectangle(0, 0, 2560, 1080) },
        { ScreenResolutions.R1080p_32_9, new Rectangle(0, 0, 3840, 1080) },
        { ScreenResolutions.R1440p_16_9, new Rectangle(322, 269, 411, 49) },
        { ScreenResolutions.R1440p_16_10, new Rectangle(0, 0, 2560, 1600) },
        { ScreenResolutions.R1440p_21_9, new Rectangle(0, 0, 3440, 1440) },
        { ScreenResolutions.R1440p_32_9, new Rectangle(0, 0, 5120, 1440) },
        { ScreenResolutions.R2160p_16_9, new Rectangle(0, 0, 3840, 2160) },
        { ScreenResolutions.R2160p_16_10, new Rectangle(0, 0, 3840, 2400) },
        { ScreenResolutions.R2160p_21_9, new Rectangle(0, 0, 5120, 2160) },
        { ScreenResolutions.R2160p_32_9, new Rectangle(0, 0, 7680, 2160) }
    };

    public static Rectangle GetPresetArea(this ScreenResolutions preset) => PresetAreas[preset];
}