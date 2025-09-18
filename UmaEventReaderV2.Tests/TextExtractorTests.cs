using System.Drawing;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Services;

namespace UmaEventReaderV2.Tests;

[TestFixture]
public class CaptureTests
{
    private string capturesDir = null!;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        // todo move to common
        capturesDir = UmaEventReader.GetSolutionCapturesPath();

        if (!Directory.Exists(capturesDir))
            Assert.Fail($"Captures folder not found at {capturesDir}");

        var eventDirs = Directory.GetDirectories(capturesDir);
        TestContext.Out.WriteLine($"📂 Found {eventDirs.Length} event(s) in {capturesDir}");
    }

    [TestCaseSource(nameof(GetEventFoldersWithNames))]
    public async Task VerifyEventFolder(string eventDir, string folderName)
    {
        var extractor = new TesseractTextExtractor();

        try
        {
            await VerifyEvent(eventDir, extractor);
        }
        catch
        {
            await TestContext.Out.WriteLineAsync($"❌ Test failed for event folder: {folderName}");

            throw;
        }
    }

    private static IEnumerable<TestCaseData> GetEventFoldersWithNames()
    {
        var dir = UmaEventReader.GetSolutionCapturesPath();

        if (!Directory.Exists(dir))
            yield break;

        foreach (var folder in Directory.GetDirectories(dir))
        {
            var folderName = Path.GetFileName(folder);

            if (string.Equals(folderName, "debug", StringComparison.OrdinalIgnoreCase))
                continue; // ❌ skip debug folder

            yield return new TestCaseData(folder, folderName)
                .SetName(folderName); // sets test display name to folder GUID
        }
    }

    private async static Task VerifyEvent(string eventDir, ITextExtractor extractor)
    {
        var txtPath = Path.Combine(eventDir, "eventName.txt");
        var rawFile = Directory.GetFiles(eventDir, "raw*.png").FirstOrDefault();
        var processedFile = Directory.GetFiles(eventDir, "processed*.png").FirstOrDefault();

        Assert.That(File.Exists(txtPath), Is.True, "eventName.txt missing.");
        Assert.That(rawFile, Is.Not.Null, "raw.png missing.");
        Assert.That(processedFile, Is.Not.Null, "processed.png missing.");

        var eventName = await File.ReadAllTextAsync(txtPath);
        Assert.That(eventName, Is.Not.Empty, "eventName.txt is empty.");

        using var rawImg = new Bitmap(rawFile);
        using var procImg = new Bitmap(processedFile);

        var rawText = await extractor.ExtractTextAsync(rawImg);
        var procText = await extractor.ExtractTextAsync(procImg);

        await TestContext.Out.WriteLineAsync($"✅ Event: {eventName}");
        await TestContext.Out.WriteLineAsync($"   Raw OCR: {rawText.Text}");
        await TestContext.Out.WriteLineAsync($"   Raw bmp: {rawFile}");
        await TestContext.Out.WriteLineAsync($"   Proc OCR: {procText.Text}");
        await TestContext.Out.WriteLineAsync($"   Proc bmp: {processedFile}");

        Assert.That(
            eventName,
            Is.EqualTo(rawText.Text).IgnoreCase
                .Or
                .EqualTo(procText.Text).IgnoreCase,
            $"OCR did not detect '{eventName}' in either raw or processed image"
        );
    }
}