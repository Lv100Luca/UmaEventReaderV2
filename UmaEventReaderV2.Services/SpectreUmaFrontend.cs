using Spectre.Console;
using Spectre.Console.Rendering;
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

        UpdatePanel(GetEventArea, "", "Event Area");
        UpdatePanel(GetCareerArea, "Placeholder", "Career Info");
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
                    Thread.Sleep(250);
                }
            });
    }

    private Layout GetEventArea => layout[EventArea];
    private Layout GetCareerArea => layout["Right"][CareerArea];
    private Layout GetLogsArea => layout["Right"][LogsArea];

    private void ShowEvent(UmaEventEntity umaEvent)
    {
        UpdatePanel(GetEventArea, umaEvent.ToString(), "Event Area", horizontalAlignment: HorizontalAlignment.Center);
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
        VerticalAlignment verticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        UpdatePanel(area, new Markup(Markup.Escape(text)), header, verticalAlignment, horizontalAlignment);
    }

    private void UpdatePanel(Layout area,
        IRenderable content,
        string header = "",
        VerticalAlignment verticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        var aligned = horizontalAlignment switch
        {
            HorizontalAlignment.Left => Align.Left(content, verticalAlignment),
            HorizontalAlignment.Center => Align.Center(content, verticalAlignment),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null)
        };

        area.Update(new Panel(aligned)
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader(header, Justify.Center)
        }.Expand());

        // context?.Refresh();
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