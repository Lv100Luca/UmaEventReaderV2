using System.Drawing;
using UmaEventReaderV2.Models;

namespace UmaEventReaderV2.Abstractions;

public interface IScreenshotAreaProvider
{
    void UpdateBaseArea(Rectangle area);

    IEnumerable<ScreenshotArea> GetAllAreas();
}