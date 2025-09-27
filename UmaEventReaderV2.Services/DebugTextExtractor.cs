using System.Drawing;
using Microsoft.Extensions.Options;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public class DebugTextExtractor(IOptions<DebugTextExtractorOptions> options) : ITextExtractor
{
    private string Result => options.Value.Result;
    private float Confidence => options.Value.Confidence;

    public TextExtractorResult ExtractText(Bitmap bmpImage, bool raw = false)
    {
        return new TextExtractorResult
        {
            Text = Result,
            Metadata = new TextExtractorResultMetadata
            {
                MeanConfidence = Confidence
            }
        };
    }
}

public class DebugTextExtractorOptions
{
    public required string Result { get; set; }
    public float Confidence { get; set; } = 1f;
}