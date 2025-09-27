using System.Text;

namespace UmaEventReaderV2.Models;

public class TextExtractorResult
{
    public required string Text { get; init; }
    public TextExtractorResultMetadata Metadata { get; init; } = new();

    public static TextExtractorResult With(string text)
    {
        return new TextExtractorResult
        {
            Text = text,
            Metadata = new TextExtractorResultMetadata { MeanConfidence = 1f }
        };
    }

    override public string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Result:")
            .AppendLine($"  - '{Text}'")
            .AppendLine($"  - {Metadata.MeanConfidence}");

        return sb.ToString();
    }
}