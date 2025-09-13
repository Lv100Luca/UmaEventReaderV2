using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

/// <summary>
/// Main running loop of the application
/// </summary>
public class UmaEventReader(IUmaEventService eventService)
{
    public void Run()
    {
        var text = "i would";

        var events = eventService.GetAllWhereChoiceTextIsLike(text);

        foreach (var e in events)
            Console.WriteLine(e);
    }
}