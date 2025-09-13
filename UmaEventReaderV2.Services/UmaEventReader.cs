using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

/// <summary>
/// Main running loop of the application
/// </summary>
public class UmaEventReader(IUmaEventRepository repository)
{
    public void Run()
    {
        var text = "i would";

        var events = repository.GetAllByChoiceText(text);

        foreach (var e in events)
            Console.WriteLine(e);
    }
}