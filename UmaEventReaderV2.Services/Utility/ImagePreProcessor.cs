using System.Drawing;

namespace UmaEventReaderV2.Services.Utility;

public class ImagePreProcessor
{
    public static Bitmap Process(Bitmap input, bool skipBorder = false, bool skipInvert = false)
    {
        if (!skipInvert)
            input = IsolateText(input);

        if (!skipBorder)
            input = AddBorder(input, 5, Color.Black);

        return input;
    }

    public static Bitmap IsolateText(Bitmap input, byte brightnessThreshold = 200)
    {
        var output = new Bitmap(input.Width, input.Height);

        for (var y = 0; y < input.Height; y++)
        {
            for (var x = 0; x < input.Width; x++)
            {
                var pixel = input.GetPixel(x, y);

                // Calculate brightness (0–255)
                var brightness = (byte)((pixel.R + pixel.G + pixel.B) / 3);

                // Background/shadow → after invert, make it white
                // This was white text → after invert, make it black
                output.SetPixel(x, y, brightness >= brightnessThreshold ? Color.Black : Color.White);
            }
        }

        return output;
    }

    public static Bitmap AddBorder(Bitmap src, int borderSize, Color color)
    {
        var newWidth = src.Width + borderSize * 2;
        var newHeight = src.Height + borderSize * 2;

        var bordered = new Bitmap(newWidth, newHeight);

        using var g = Graphics.FromImage(bordered);

        // Fill background with border color
        g.Clear(color);

        // Draw original image inside the border
        g.DrawImage(src, new Rectangle(borderSize, borderSize, src.Width, src.Height));

        return bordered;
    }
}