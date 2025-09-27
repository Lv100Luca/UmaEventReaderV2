// using System.Drawing;
// using System.Windows.Forms;
// using UmaEventReaderV2.Abstractions;
// using UmaEventReaderVs.WinForms;

// namespace UmaEventReaderV2.Services;

// public class ScreenshotAreaSelector(SelectAreaOverlay selectAreaOverlay, EventAreaOffsetProvider offsetProvider)
//     : IScreenshotAreaProvider
// {
//     private Rectangle? SelectedArea { get; set; }
//
//     public Rectangle GetEventArea()
//     {
//         if (SelectedArea is null)
//             SelectedArea = selectAreaOverlay.ShowSelection();
//
//         // should it still be null, throw here
//         if (SelectedArea == null)
//             throw new InvalidOperationException("No area selected");
//
//         return (Rectangle)SelectedArea;
//     }
//
//     public Rectangle GetFallbackEventArea()
//     {
//         if (SelectedArea == null)
//             throw new InvalidOperationException("No area selected");
//
//         // cant be null here but its schizo
//         return offsetProvider.OffsetRectangle((Rectangle)SelectedArea);
//     }
// }