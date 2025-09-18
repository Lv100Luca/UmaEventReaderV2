// See https://aka.ms/new-console-template for more information

using Spectre.Console;

// Create the layout
var layout = new Layout("Root")
    .SplitColumns(
        new Layout("Left"),
        new Layout("Right")
            .SplitRows(
                new Layout("Top"),
                new Layout("Bottom")));

// Update the left column
layout["Left"].Update(
    new Panel(
            Align.Center(
                new Markup("Hello [blue]World![/]"),
                VerticalAlignment.Middle))
        .Expand());

// Render the layout
await AnsiConsole.Live(layout).StartAsync(async a =>
{
    while (true)
    {
        layout["Left"].Update(
            new Panel(
                Align.Center(
                    new Markup("Hello [blue]World![/]"),
                    VerticalAlignment.Middle)
            ).Expand()
        );

        a.Refresh();

        await Task.Delay(2000);

        layout["Left"].Update(
            new Panel(
                Align.Center(
                    new Markup("Hello [RED]adgfasdgfasgfasd![/]"),
                    VerticalAlignment.Middle)
            ).Expand()
        );

        a.Refresh();

        await Task.Delay(2000);
    }
});
