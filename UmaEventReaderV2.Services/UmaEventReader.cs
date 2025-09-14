using UmaEventReaderV2.Abstractions;

namespace UmaEventReaderV2.Services;

/// <summary>
/// Main running loop of the application
/// </summary>
public class UmaEventReader(IUmaEventService eventService, IScreenshotAreaProvider screenshotAreaProvider)
{
    public void Run()
    {
        var area = screenshotAreaProvider.GetEventArea();

        Console.Out.WriteLine("Area" + area);;

        // var text = "i would";
        //
        // var events = eventService.GetAllWhereChoiceTextIsLike(text);
        //
        // foreach (var e in events)
        //     Console.WriteLine(e);
    }
}