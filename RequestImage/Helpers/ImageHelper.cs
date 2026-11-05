using SkiaSharp;

namespace RequestImage.Helpers;

public class ImageHelper : IImageHelper
{
    public byte[] AddTextToImage(Stream imageStream,
        params (string text, (float x, float y) position, int fontSize, string colorHex)[] texts)
    {
        using var image = SKBitmap.Decode(imageStream)
            ?? throw new InvalidOperationException("Unable to decode the downloaded image.");
        using var canvas = new SKCanvas(image);
        using var gradient = SKShader.CreateLinearGradient(
            new SKPoint(0, 0),
            new SKPoint(0, image.Height * 0.65f),
            [new SKColor(0, 0, 0, 200), SKColors.Transparent],
            SKShaderTileMode.Clamp);
        using var overlay = new SKPaint { Shader = gradient };

        canvas.DrawRect(0, 0, image.Width, image.Height, overlay);

        using var typeface = SKTypeface.FromFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSans.ttf"))
            ?? throw new InvalidOperationException("Unable to load the bundled DejaVu Sans font.");
        using var paint = new SKPaint { IsAntialias = true };
        using var shadow = new SKPaint { IsAntialias = true, Color = SKColors.Black };

        foreach (var (text, (x, y), fontSize, colorHex) in texts)
        {
            using var font = new SKFont(typeface, fontSize);
            paint.Color = SKColor.Parse(colorHex);
            var baseline = y - font.Metrics.Ascent;

            foreach (var line in WrapText(text, font, image.Width - x - 10))
            {
                canvas.DrawText(line, x + 2, baseline + 2, SKTextAlign.Left, font, shadow);
                canvas.DrawText(line, x, baseline, SKTextAlign.Left, font, paint);
                baseline += font.Spacing;
            }
        }

        canvas.Flush();
        using var output = SKImage.FromBitmap(image);
        using var data = output.Encode(SKEncodedImageFormat.Png, 100)
            ?? throw new InvalidOperationException("Unable to encode the weather image as PNG.");

        return data.ToArray();
    }

    private static IEnumerable<string> WrapText(string text, SKFont font, float width)
    {
        foreach (var paragraph in text.Replace("\r\n", "\n").Split('\n'))
        {
            var line = string.Empty;

            foreach (var word in paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            {
                var candidate = line.Length == 0 ? word : $"{line} {word}";

                if (line.Length > 0 && font.MeasureText(candidate) > width)
                {
                    yield return line;
                    line = word;
                }
                else
                {
                    line = candidate;
                }
            }

            yield return line;
        }
    }
}
