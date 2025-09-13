namespace UmaEventReaderV2.Models;

public class TextExtractorResult
{
    public required string Text { get; init; }
    public TextExtractorResultMetadata Metadata { get; init; } = new();
}