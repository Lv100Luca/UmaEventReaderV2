using Spectre.Console;
using UmaEventReaderV2.Models.Entities;
using UmaEventReaderV2.Services;

public class SpectreUmaFrontend
{
    private Layout layout;
    private readonly List<string> logs = new();

    private const string Root = "Root";
    private const string EventArea = "Event";
    private const string CareerArea = "Career";
    private const string LogsArea = "Logs";

    public SpectreUmaFrontend(UmaEventReader reader)
    {
        layout = InitializeLayout();

        UpdatePanel(GetEventArea, "", "Event Area", HorizontalAlignment.Center);
        UpdatePanel(GetCareerArea, "", "Career Info", HorizontalAlignment.Center);
        UpdatePanel(GetLogsArea, "", "Logs");

        reader.OnLog += Log;
        reader.OnEventFound += ShowEvent;
    }


    public void Run()
    {
        AnsiConsole.Live(layout)
            .AutoClear(false) // keeps panel after exit
            .Start(ctx =>
            {
                while (true)
                {
                    ctx.Refresh();
                    Thread.Sleep(50);
                }
            });
    }

    private Layout GetEventArea => layout[EventArea];
    private Layout GetCareerArea => layout["Right"][CareerArea];
    private Layout GetLogsArea => layout["Right"][LogsArea];

    private void ShowEvent(UmaEventEntity umaEvent)
    {
        UpdatePanel(GetEventArea, umaEvent.ToString(), "Event Area", HorizontalAlignment.Center);
    }

    public void ShowCareer(string careerInfo)
    {
        UpdatePanel(GetCareerArea, careerInfo, "Career Info", HorizontalAlignment.Center);
    }

    public void Log(string message)
    {
        logs.Add(message);
        if (logs.Count > 15) logs.RemoveAt(0);

        // Build log table
        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn("Log");

        foreach (var log in logs.AsEnumerable().Reverse())
            table.AddRow(log);

        UpdatePanel(GetLogsArea, table, "Logs");
    }

    private void UpdatePanel(Layout area,
        string text,
        string header = "",
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var panel = horizontalAlignment switch
        {
            HorizontalAlignment.Left => new Panel(Align.Left(new Markup(text), verticalAlignment)),
            HorizontalAlignment.Center => new Panel(Align.Center(new Markup(text), verticalAlignment)),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
        };

        panel.Border = BoxBorder.Rounded;
        panel.Header = new PanelHeader(header, Justify.Center);
        area.Update(panel.Expand());
    }

    private void UpdatePanel(Layout area,
        Table content,
        string header = "",
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
        VerticalAlignment verticalAlignment = VerticalAlignment.Top)
    {
        var panel = horizontalAlignment switch
        {
            HorizontalAlignment.Left => new Panel(Align.Left(content, verticalAlignment)),
            HorizontalAlignment.Center => new Panel(Align.Center(content, verticalAlignment)),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
        };

        panel.Border = BoxBorder.Rounded;
        panel.Header = new PanelHeader(header, Justify.Center);
        area.Update(panel.Expand());
    }


    private static Layout InitializeLayout()
    {
        return new Layout(Root)
            .SplitColumns(
                new Layout("Left")
                    .SplitRows(
                        new Layout(EventArea).Ratio(4)
                    ),
                new Layout("Right").Size(30)
                    .SplitRows(
                        new Layout(CareerArea).Size(5),
                        new Layout(LogsArea)
                    )
            );
    }
}