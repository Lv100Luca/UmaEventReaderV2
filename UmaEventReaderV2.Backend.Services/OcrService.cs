using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Services.Utility;

namespace UmaEventReaderV2.Services;

public class OcrService(IScreenshotProvider screenshotProvider, ITextExtractor textExtractor)
{
    public TextExtractorResult ExtractText(ScreenshotArea area)
    {
        var raw = screenshotProvider.TakeScreenshot(area.Area);

        if (raw is null)
            return TextExtractorResult.Empty;

        var processed = ImagePreProcessor.Process(raw, skipBorder: true);

        var result = textExtractor.ExtractText(processed);

        result.Metadata.RawImage = raw;

        return result;
    }
}