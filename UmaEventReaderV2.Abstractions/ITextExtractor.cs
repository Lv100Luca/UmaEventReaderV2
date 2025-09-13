using System.Drawing;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Abstractions;

public interface ITextExtractor
{
    public Task<TextExtractorResult> ExtractTextAsync(Bitmap bmpImage);
}