using Spectre.Console;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.Entities;

public class SpectreUmaFrontend : IUmaFrontend
{
    private readonly Layout layout;
    private readonly List<string> logs = new();
    private readonly object searchLock = new();

    private string searchQuery = string.Empty;
    private bool isSearching;

    private const string Root = "Root";
    private const string EventArea = "Event";
    private const string CareerArea = "Career";
    private const string LogsArea = "Logs";

    private enum InputMode
    {
        Search,
        Career
    }

    private InputMode currentInputMode = InputMode.Search;
    private string careerInput = string.Empty;

    public SpectreUmaFrontend()
    {
        layout = new Layout(Root)
            .SplitColumns(
                new Layout("Left")
                    .SplitRows(
                        new Layout(EventArea).Ratio(4),
                        new Layout("Search").Size(3)
                    ),
                new Layout("Right").Size(50)
                    .SplitRows(
                        new Layout(CareerArea).Size(5),
                        new Layout(LogsArea)
                    )
            );

        // layout[EventArea].Update(
        //     new Panel(Align.Center(new Markup("Events"), VerticalAlignment.Middle))
        //     {
        //         Border = BoxBorder.Rounded,
        //         Header = new PanelHeader("Event Area", Justify.Center)
        //     }.Expand()
        // );
        //
        // layout["Right"][CareerArea].Update(
        //     new Panel(Align.Center(new Markup($"Career Info"), VerticalAlignment.Top))
        //     {
        //         Border = BoxBorder.Rounded,
        //         Header = new PanelHeader("Career info", Justify.Center)
        //     }.Expand()
        // );
        //
        // layout["Right"][LogsArea].Update(
        //     new Panel("Logs...")
        //     {
        //         Border = BoxBorder.Rounded,
        //         Header = new PanelHeader("Logs", Justify.Center)
        //     }
        // );
        //
        // layout["Left"]["Search"].Update(new Panel("Search:").Expand());



        // Start live renderer
        Task.Run(() =>
        {
            AnsiConsole.Live(layout).Start(ctx =>
            {
                while (true)
                {
                    ctx.Refresh();
                    Thread.Sleep(100);
                }
            });
        });

        StartInputCapture();
    }

    private void StartInputCapture()
    {
        Task.Run(async () =>
        {
            var lastInput = DateTime.UtcNow;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(intercept: true);
                    lastInput = DateTime.UtcNow;

                    lock (searchLock)
                    {
                        isSearching = true;

                        if (key.Key == ConsoleKey.Tab)
                        {
                            // Toggle input mode
                            currentInputMode = currentInputMode == InputMode.Search ? InputMode.Career : InputMode.Search;
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            if (currentInputMode == InputMode.Career && careerInput.Length > 0)
                            {
                                // Confirm career input → top-right panel
                                ShowCareerAsync(careerInput);
                                careerInput = string.Empty;
                                currentInputMode = InputMode.Search; // return to search
                            }
                        }
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (currentInputMode == InputMode.Search && searchQuery.Length > 0)
                                searchQuery = searchQuery[..^1];
                            else if (currentInputMode == InputMode.Career && careerInput.Length > 0)
                                careerInput = careerInput[..^1];
                        }
                        else if (!char.IsControl(key.KeyChar))
                        {
                            if (currentInputMode == InputMode.Search)
                                searchQuery += key.KeyChar;
                            else if (currentInputMode == InputMode.Career)
                                careerInput += key.KeyChar;
                        }
                    }

                    await UpdateInputBarThreadSafe();
                }

                if ((DateTime.UtcNow - lastInput).TotalSeconds > 1)
                {
                    lock (searchLock) isSearching = false;
                }

                await Task.Delay(50);
            }
        });
    }


    public Task ShowEventAsync(UmaEventEntity umaEvent)
    {
        layout[EventArea].Update(
            new Panel(Align.Center(new Markup($"{umaEvent}"), VerticalAlignment.Middle))
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("Event Area", Justify.Center)
            }.Expand()
        );

        return Task.CompletedTask;
    }

    public Task ShowCareerAsync(string careerInfo)
    {
        layout["Right"][CareerArea].Update(
            new Panel(Align.Center(new Markup($"{careerInfo}"), VerticalAlignment.Top))
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("Career info", Justify.Center)
            }.Expand()
        );

        return Task.CompletedTask;
    }

    public Task LogAsync(string message)
    {
        logs.Add(message);
        if (logs.Count > 15) logs.RemoveAt(0);

        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn("Log");
        foreach (var log in logs) table.AddRow(log);

        layout["Right"][LogsArea].Update(new Panel(table)
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("Log", Justify.Center)
            }.Expand()
        );

        return Task.CompletedTask;
    }

    private Task UpdateInputBarThreadSafe()
    {
        string inputCopy;
        bool searching;
        InputMode mode;

        lock (searchLock)
        {
            inputCopy = currentInputMode == InputMode.Search ? searchQuery : careerInput;
            searching = isSearching;
            mode = currentInputMode;
        }

        // Dynamic label
        string label = mode switch
        {
            InputMode.Search => searching ? "[green]Search:[/]" : "Search:",
            InputMode.Career => "[yellow]Career:[/]",
            _ => "Input:"
        };

        layout["Left"]["Search"].Update(
            new Panel($"{label} {inputCopy}") { Border = BoxBorder.Rounded }.Expand()
        );

        return Task.CompletedTask;
    }

    public string GetSearchQuery()
    {
        lock (searchLock) return searchQuery;
    }

    public void ResetSearchQuery()
    {
        lock (searchLock)
        {
            // Only reset if not currently typing and input mode is Search
            if (isSearching || currentInputMode != InputMode.Search)
                return;

            searchQuery = string.Empty;
        }

        _ = UpdateInputBarThreadSafe(); // update both bars
    }

    public bool IsSearching()
    {
        lock (searchLock) return isSearching;
    }

    private void UpdatePanel(string area, string text, VerticalAlignment alignment = VerticalAlignment.Top, string header = null)
    {
        layout[area].Update(
            new Panel(Align.Center(new Markup(text), alignment))
            {
                Border = BoxBorder.Rounded,
                Header = header != null ? new PanelHeader(header, Justify.Center) : null
            }.Expand()
        );
    }
}