using System.Windows.Forms;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.Entities;

namespace UmaEventReaderV2.Services;

using Spectre.Console;

public class SpectreUmaFrontend : IUmaFrontend
{
    private readonly Layout layout;
    private readonly List<string> logs = new();

    public SpectreUmaFrontend()
    {
        layout = new Layout("Root")
            .SplitRows(
                new Layout("Event").Size(5), // event area (fixed height)
                new Layout("Bottom")
                    .SplitColumns(
                        new Layout("Career").Size(30), // narrower left side
                        new Layout("Logs")             // logging takes the rest
                    )
            );

        // Initial panels
        layout["Event"].Update(new Panel("Waiting for events..."));
        layout["Bottom"]["Career"].Update(new Panel("Career info..."));
        layout["Bottom"]["Logs"].Update(new Panel("Logs..."));

        // Run live renderer in background
        Task.Run(() =>
        {
            AnsiConsole.Live(layout).Start(ctx =>
            {
                while (true)
                {
                    ctx.Refresh();
                    Thread.Sleep(200); // Refresh interval
                }
            });
        });
    }

    public Task ShowEventAsync(UmaEventEntity umaEvent)
    {
        layout["Event"].Update(new Panel($"[yellow]{umaEvent}[/]") { Border = BoxBorder.Rounded });
        return Task.CompletedTask;
    }

    public Task ShowCareerAsync(string careerInfo)
    {
        layout["Bottom"]["Career"].Update(new Panel($"[green]{careerInfo}[/]") { Border = BoxBorder.Rounded });
        return Task.CompletedTask;
    }

    public Task LogAsync(string message)
    {
        logs.Add(message);
        if (logs.Count > 10) // keep only last 10
            logs.RemoveAt(0);

        var table = new Table().Border(TableBorder.None);
        table.AddColumn(""); // single column
        foreach (var log in logs)
            table.AddRow(log);

        layout["Bottom"]["Logs"].Update(new Panel(table) { Border = BoxBorder.Rounded });
        return Task.CompletedTask;
    }
}