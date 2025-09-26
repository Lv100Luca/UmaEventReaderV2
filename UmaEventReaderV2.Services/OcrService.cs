using System.Drawing;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Services.Utility;

namespace UmaEventReaderV2.Services;

public class OcrService(IScreenshotProvider screenshotProvider, ITextExtractor textExtractor)
{
    public async Task<TextExtractorResult> ExtractText(Rectangle area)
    {
        var raw = screenshotProvider.TakeScreenshot(area);
        var processed = ImagePreProcessor.Process(raw, skipBorder: true);

        var result = await textExtractor.ExtractTextAsync(processed);

        result.Metadata.RawImage = raw;

        return result;
    }
}
