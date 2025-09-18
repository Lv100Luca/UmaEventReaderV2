using Spectre.Console;
using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models.Entities;

public class SpectreUmaFrontend : IUmaFrontend
{
    private readonly Layout layout;
    private readonly List<string> logs = new();
    private readonly object searchLock = new();

    private string searchQuery = string.Empty;
    private string typingBuffer = string.Empty; // user types here first

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

        UpdatePanel(GetEventArea, "", "Event Area", horizontalAlignment: HorizontalAlignment.Center);
        UpdatePanel(GetCareerArea, "", "Career Info", horizontalAlignment: HorizontalAlignment.Center);
        UpdatePanel(GetLogsArea, "", "Logs");
        UpdatePanel(GetSearchArea, "Search: ");

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

        StartInputCapture();
    }

    private Layout GetEventArea => layout[EventArea];
    private Layout GetCareerArea => layout["Right"][CareerArea];
    private Layout GetLogsArea => layout["Right"][LogsArea];
    private Layout GetSearchArea => layout["Left"]["Search"];

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
                            currentInputMode = currentInputMode == InputMode.Search ? InputMode.Career : InputMode.Search;
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            if (currentInputMode == InputMode.Career && careerInput.Length > 0)
                            {
                                ShowCareerAsync(careerInput);
                                careerInput = string.Empty;
                                currentInputMode = InputMode.Search;
                            }
                            else if (currentInputMode == InputMode.Search)
                            {
                                // Optional: confirm search immediately on Enter
                                searchQuery = typingBuffer;
                            }
                        }
                        else if (key.Key == ConsoleKey.Backspace)
                        {
                            if (currentInputMode == InputMode.Search && typingBuffer.Length > 0)
                                typingBuffer = typingBuffer[..^1];
                            else if (currentInputMode == InputMode.Career && careerInput.Length > 0)
                                careerInput = careerInput[..^1];
                        }
                        else if (!char.IsControl(key.KeyChar))
                        {
                            if (currentInputMode == InputMode.Search)
                                typingBuffer += key.KeyChar;
                            else if (currentInputMode == InputMode.Career)
                                careerInput += key.KeyChar;
                        }
                    }

                    await UpdateInputBarThreadSafe();
                }

                // Debounce: commit typingBuffer to searchQuery after 1 second of inactivity
                if (currentInputMode == InputMode.Search && (DateTime.UtcNow - lastInput).TotalSeconds > 1)
                {
                    lock (searchLock)
                    {
                        if (searchQuery != null && typingBuffer != searchQuery)
                            searchQuery = typingBuffer; // commit

                        isSearching = false;
                    }
                }

                await Task.Delay(50);
            }
        });
    }

    public Task ShowEventAsync(UmaEventEntity umaEvent)
    {
        UpdatePanel(GetEventArea, $"{umaEvent}", "Event Area", horizontalAlignment: HorizontalAlignment.Center);

        return Task.CompletedTask;
    }

    public Task ShowCareerAsync(string careerInfo)
    {
        UpdatePanel(GetCareerArea, careerInfo, "Career Info", horizontalAlignment: HorizontalAlignment.Center);

        return Task.CompletedTask;
    }

    public Task LogAsync(string message)
    {
        logs.Add(message);
        if (logs.Count > 15) logs.RemoveAt(0);

        var table = new Table().Border(TableBorder.None).HideHeaders();
        table.AddColumn("Log");
        foreach (var log in logs) table.AddRow(log);

        UpdatePanel(GetLogsArea, table, "Logs");

        return Task.CompletedTask;
    }

    private Task UpdateInputBarThreadSafe()
    {
        string inputCopy;
        bool searching;
        InputMode mode;

        lock (searchLock)
        {
            inputCopy = currentInputMode == InputMode.Search ? typingBuffer : careerInput;
            searching = isSearching;
            mode = currentInputMode;
        }

        var label = mode switch
        {
            InputMode.Search => searching ? "[green]Search:[/]" : "Search:",
            InputMode.Career => "[yellow]Career:[/]",
            _ => "Input:"
        };

        UpdatePanel(GetSearchArea, $"{label} {inputCopy}", "Input");

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

    private void UpdatePanel(Layout area,
        string text,
        string header = "",
        VerticalAlignment alignment = VerticalAlignment.Top,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Left:
                area.Update(
                    new Panel(Align.Left(new Markup(text), alignment))
                    {
                        Border = BoxBorder.Rounded,
                        Header = new PanelHeader(header, Justify.Center)
                    }.Expand()
                );

                break;
            case HorizontalAlignment.Center:
                area.Update(
                    new Panel(Align.Center(new Markup(text), alignment))
                    {
                        Border = BoxBorder.Rounded,
                        Header = new PanelHeader(header, Justify.Center)
                    }.Expand()
                );

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null);
        }
    }

    private void UpdatePanel(Layout area,
        Table content,
        string header = "",
        VerticalAlignment alignment = VerticalAlignment.Top,
        HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left)
    {
        switch (horizontalAlignment)
        {
            case HorizontalAlignment.Left:
                area.Update(
                    new Panel(Align.Left(content, alignment))
                    {
                        Border = BoxBorder.Rounded,
                        Header = new PanelHeader(header, Justify.Center)
                    }.Expand()
                );

                break;
            case HorizontalAlignment.Center:
                area.Update(
                    new Panel(Align.Center(content, alignment))
                    {
                        Border = BoxBorder.Rounded,
                        Header = new PanelHeader(header, Justify.Center)
                    }.Expand()
                );

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(horizontalAlignment), horizontalAlignment, null);
        }
    }
}