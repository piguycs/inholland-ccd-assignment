namespace RequestImage.Helpers;

public interface IImageHelper
{
    byte[] AddTextToImage(Stream imageStream,
        params (string text, (float x, float y) position, int fontSize, string colorHex)[] texts);
}
