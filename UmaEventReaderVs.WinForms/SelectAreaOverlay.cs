namespace UmaEventReaderVs.WinForms;

public class SelectAreaOverlay
{
    public Rectangle? ShowSelection()
    {
        var form = new SelectionForm();
        return form.ShowDialog() == DialogResult.OK ? form.SelectedRegion : null;
    }
}