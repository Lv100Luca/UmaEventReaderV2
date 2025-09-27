using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public static class TextValidator
{
    public static bool IsValid(TextExtractorResult result, float confidenceThreshold = 0.6f)
    {
        return result.Text.Length >= 3 &&
               !string.IsNullOrWhiteSpace(result.Text) &&
               result.Metadata.MeanConfidence >= confidenceThreshold;
    }
}