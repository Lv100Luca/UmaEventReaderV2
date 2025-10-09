using UmaEventReaderV2.Abstractions;
using UmaEventReaderV2.Models;
using UmaEventReaderV2.Services;

namespace UmaEventReaderVs.WinForms;

public class ScreenshotAreaSelector(SelectAreaOverlay selectAreaOverlay, EventAreaOffsetProvider offsetProvider)
    : IScreenshotAreaProvider
{
    private bool HasSelectedArea { get; set; }
    private Rectangle SelectedArea { get; set; }

    private void SelectArea()
    {
        var rect = selectAreaOverlay.ShowSelection();

        if (rect == null)
            throw new InvalidOperationException("No area selected");

        HasSelectedArea = true;

        SelectedArea = rect.Value;
    }

    public ScreenshotArea GetEventArea()
    {
        if (!HasSelectedArea)
            SelectArea();

        return new ScreenshotArea
        {
            Name = "Full Event Area",
            Area = SelectedArea
        };
    }

    public ScreenshotArea GetOffsetEventArea()
    {
        if (!HasSelectedArea)
            SelectArea();

        // var offsetArea = offsetProvider.OffsetRectangle(SelectedArea);

        return new ScreenshotArea
        {
            Name = "Offset Event Area",
            Area = SelectedArea
        };
    }
}