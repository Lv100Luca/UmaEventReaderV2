namespace UmaEventReaderVs.WinForms;

public class ScreenshotAreaSelector(SelectAreaOverlay selectAreaOverlay)
{
    public Rectangle SelectArea()
    {
        var rect = selectAreaOverlay.ShowSelection();

        if (rect == null)
            throw new InvalidOperationException("No area selected");

        return rect.Value;
    }
}