using System.Drawing;

namespace UmaEventReaderV2.Common.Models;

public interface ISettingsHub
{
    Task<UmaEventReaderSettings> GetSettingsAsync();
    Task SaveSettingsAsync(UmaEventReaderSettings settings);

    Task<Rectangle?> SelectNewAreaAsync();

    Task<string?> CapturePreviewScreenshotAsync(Rectangle area);
}