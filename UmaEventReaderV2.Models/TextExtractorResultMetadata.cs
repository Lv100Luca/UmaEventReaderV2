using System.Drawing;

namespace UmaEventReaderV2.Models;

public class TextExtractorResultMetadata
{
    public float MeanConfidence { get; init; } = 0.0f;
    public Bitmap? ProcessedImage { get; init; }
}