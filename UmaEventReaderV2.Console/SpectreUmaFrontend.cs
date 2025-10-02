using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;
using Spectre.Console.Rendering;
using UmaEventReaderV2.Common.Models;

namespace UmaEventReaderV2.Console;

public class SpectreUmaFrontend
{
    private readonly HubConnection hubConnection;
    private readonly Layout layout;
    private readonly List<string> logs = [];

    private const string Root = "Root";
    private const string EventArea = "Event";
    private const string CareerArea = "Career";
    private const string LogsArea = "Logs";

    public SpectreUmaFrontend(HubConnection hubConnection)
    {
        this.hubConnection = hubConnection;

        layout = InitializeLayout();

        UpdatePanel(GetEventArea, "", "Event Area");
        UpdatePanel(GetCareerArea, "Placeholder", "Career Info");
        UpdatePanel(GetLogsArea, "", "Logs");

        hubConnection.On<EventBatch>("OnEventFound", ShowEvent);
    }

    public async Task RunAsync()
    {
        await hubConnection.StartAsync();
        AnsiConsole.MarkupLine("[green]Connected to SignalR hub[/]");

        AnsiConsole.Live(layout)
            .AutoClear(false) // keeps panel after exit
            .Start(ctx =>
            {
                while (true)
                {
                    ctx.Refresh();
                    Thread.Sleep(250);
                }
                // ReSharper disable once FunctionNeverReturns
            });
    }

    private Layout GetEventArea => layout[EventArea];
    private Layout GetCareerArea => layout["Right"][CareerArea];
    private Layout GetLogsArea => layout["Right"][LogsArea];

    private void ShowEvent(EventBatch umaEvent)
    {
        UpdatePanel(GetEventArea, umaEvent.Events.FirstOrDefault()?.Name, "Event Area",
            horizontalAlignment: HorizontalAlignment.Center);
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
        string? text,
        string header = "",
        VerticalAlignment verticalAlignment = VerticalAlignment.Top,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        UpdatePanel(area, new Markup(Markup.Escape(text ?? "No Text")), header, verticalAlignment, horizontalAlignment);
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