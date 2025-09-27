using System.Drawing;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public class DebugTextExtractor
(
    string result,
    float confidence = 1f
) : ITextExtractor
{
    public TextExtractorResult ExtractText(Bitmap bmpImage, bool raw = false)
    {
        return new TextExtractorResult
        {
            Text = result,
            Metadata = new TextExtractorResultMetadata
            {
                MeanConfidence = confidence
            }
        };
    }
}

public class DebugTextExtractorOptions
{
    public required string Result { get; set; }
    public float Confidence { get; set; } = 1f;
}