using System.ComponentModel;
namespace UmaEventReader;

using System.Drawing;
using System.Windows.Forms;

public class SelectionForm : Form
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Rectangle SelectedRegion { get; private set; }
    private Point _startPoint;
    private Rectangle _currentRect;

    public SelectionForm()
    {
        FormBorderStyle = FormBorderStyle.None;
        BackColor = Color.LightGray;
        Opacity = 0.25;
        TopMost = true;
        WindowState = FormWindowState.Maximized;
        DoubleBuffered = true;
        Cursor = Cursors.Cross;

        MouseDown += SelectionForm_MouseDown;
        MouseMove += SelectionForm_MouseMove;
        MouseUp += SelectionForm_MouseUp;
    }

    private void SelectionForm_MouseDown(object sender, MouseEventArgs e)
    {
        _startPoint = e.Location;
        _currentRect = new Rectangle(e.Location, new Size(0, 0));
    }

    private void SelectionForm_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;

        var x = Math.Min(e.X, _startPoint.X);
        var y = Math.Min(e.Y, _startPoint.Y);
        var width = Math.Abs(e.X - _startPoint.X);
        var height = Math.Abs(e.Y - _startPoint.Y);

        _currentRect = new Rectangle(x, y, width, height);
        Invalidate(); // triggers repaint
    }

    private void SelectionForm_MouseUp(object sender, MouseEventArgs e)
    {
        SelectedRegion = _currentRect;
        DialogResult = DialogResult.OK;
        Close();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_currentRect == Rectangle.Empty)
            return;

        using var pen = new Pen(Color.Red, 2);
        e.Graphics.DrawRectangle(pen, _currentRect);
    }
}