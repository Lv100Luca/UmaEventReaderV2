using Spectre.Console;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.Entities;

public class SpectreUmaFrontend : IUmaFrontend
{
    private readonly Layout layout;
    private readonly List<string> logs = new();
    private readonly object searchLock = new(); // lock for thread-safety
    private string searchQuery = string.Empty;
    private bool isSearching;

    private const string Root = "Root";
    private const string EventArea = "Event";
    private const string CareerArea = "Career";
    private const string LogsArea = "Logs";

    public SpectreUmaFrontend()
    {
        layout = new Layout(Root)
            .SplitColumns(
                new Layout("Left") // left column: Events + Search
                    .SplitRows(
                        new Layout(EventArea).Ratio(4),   // Event area (largest)
                        new Layout("Search").Size(3)      // Search bar (fixed height)
                    ),
                new Layout("Right").Size(50) // right column: Career + Logs
                    .SplitRows(
                        new Layout(CareerArea).Size(5), // Career info
                        new Layout(LogsArea)            // Logs
                    )
            );

        // Initial state
        layout[EventArea].Update(new Panel("Waiting for events...").Expand());
        layout["Left"]["Search"].Update(new Panel("Search:").Expand());
        layout["Right"][CareerArea].Update(new Panel("Career info...").Expand());
        layout["Right"][LogsArea].Update(new Panel("Logs...").Expand());

        // Start live renderer
        Task.Run(() =>
        {
            AnsiConsole.Live(layout).Start(ctx =>
            {
                while (true)
                {
                    ctx.Refresh();
                    Thread.Sleep(50);
                }
            });
        });

        var lastInput = DateTime.UtcNow;

        // Start background input capture (non-blocking search)
        Task.Run(async () =>
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);

                    lock (searchLock)
                    {
                        isSearching = true;

                        if (key.Key == ConsoleKey.Backspace && searchQuery.Length > 0)
                            searchQuery = searchQuery[..^1];
                        else if (!char.IsControl(key.KeyChar))
                            searchQuery += key.KeyChar;
                    }

                    await UpdateSearchBarThreadSafe(); // uses a snapshot copy inside the lock
                }

                await Task.Delay(50);
            }
        });
    }

    public Task ShowEventAsync(UmaEventEntity umaEvent)
    {
        layout[EventArea].Update(
            new Panel(
                Align.Center(new Markup($"{umaEvent}"), VerticalAlignment.Middle)
            ).Expand()
        );

        return Task.CompletedTask;
    }

    public Task ShowCareerAsync(string careerInfo)
    {
        layout["Right"][CareerArea].Update(
            new Panel(
                Align.Center(new Markup($"{careerInfo}"), VerticalAlignment.Top)
            ).Expand()
        );

        return Task.CompletedTask;
    }

    public Task LogAsync(string message)
    {
        logs.Add(message);
        if (logs.Count > 15)
            logs.RemoveAt(0);

        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn("Log");
        foreach (var log in logs)
            table.AddRow(log);

        layout["Right"][LogsArea].Update(
            new Panel(table).Expand()
        );

        return Task.CompletedTask;
    }

    // Thread-safe search bar update
    private Task UpdateSearchBarThreadSafe()
    {
        string queryCopy;
        lock (searchLock)
        {
            queryCopy = searchQuery;
        }

        layout["Left"]["Search"].Update(
            new Panel($"[yellow]Search:[/] {queryCopy}") { Border = BoxBorder.Rounded }.Expand()
        );

        return Task.CompletedTask;
    }

    // Thread-safe getter
    public string GetSearchQuery()
    {
        lock (searchLock)
        {
            return searchQuery;
        }
    }

    // Thread-safe reset
    public void ResetSearchQuery()
    {
        lock (searchLock)
        {
            if (isSearching)
                return; // do nothing if the user is typing

            searchQuery = string.Empty;
        }

        _ = UpdateSearchBarThreadSafe(); // refresh the panel
    }


    public bool IsSearching()
    {
        lock (searchLock)
        {
            return isSearching;
        }
    }
}
