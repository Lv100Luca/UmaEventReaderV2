using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using Tesseract;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Services;

public partial class TesseractTextExtractor : ITextExtractor
{
    private const string TesseractTraineeDataPath = "tessdata";
    private const string CharWhitelist = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,!?'()#- ";
    private const PageSegMode DefaultPageSegMode = PageSegMode.SingleLine;

    private TesseractEngine Engine { get; init; }

    public TesseractTextExtractor()
    {
        Engine = InitEngine();
        ConfigureEngine();
    }

    public Task<TextExtractorResult> ExtractTextAsync(Bitmap bmpImage, bool raw = false)
    {
        var pix = PixConverter.ToPix(bmpImage);

        using var page = Engine.Process(pix);

        var text = page.GetText();

        if (!raw)
            text = Clean(text);

        var conf = page.GetMeanConfidence();

        var sb = new StringBuilder();

        sb.AppendLine("Result")
            .AppendLine($" - '{text}'")
            .AppendLine($" - {conf}");

        // Console.Out.WriteLine(sb.ToString());

        var result = new TextExtractorResult
        {
            Text = text,
            Metadata = new TextExtractorResultMetadata
            {
                MeanConfidence = conf,
                ProcessedImage = bmpImage
            }
        };

        return Task.FromResult(result);
    }

    private static TesseractEngine InitEngine()
    {
        var tessdataPath = Path.Combine(AppContext.BaseDirectory, TesseractTraineeDataPath);

        return new TesseractEngine(tessdataPath, "eng", EngineMode.TesseractAndLstm);
    }

    private void ConfigureEngine()
    {
        Engine.SetVariable("tessedit_char_whitelist", CharWhitelist);
        Engine.SetVariable("debug_file", "NUL");
        Engine.DefaultPageSegMode = DefaultPageSegMode;
    }

    // todo: maybe move to service?
    private static string Clean(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Replace("\r\n", "").Replace("\r", "").Trim();
        input = NormalizeSpaces().Replace(input, " "); // normalize spaces
        // input = NonAlphaNum().Replace(input, ""); // trim non-alphanum edges

        return input;
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex NormalizeSpaces();

    [GeneratedRegex(@"^[^\w\d]+|[^\w\d]+$")]
    private static partial Regex NonAlphaNum();
}