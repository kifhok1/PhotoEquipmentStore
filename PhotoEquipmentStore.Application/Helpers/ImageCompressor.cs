using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

public static class ImageCompressor
{
    private const long MaxSizeBytes    = 2 * 1024 * 1024;
    private const long TargetSizeBytes = 500 * 1024;

    public static byte[]? CompressIfNeeded(byte[]? imageBytes)
    {
        if (imageBytes is null || imageBytes.Length == 0)
            return imageBytes;

        if (imageBytes.Length <= MaxSizeBytes)
            return imageBytes;

        return Compress(imageBytes);
    }

    private static byte[] Compress(byte[] imageBytes)
    {
        using var inputStream = new MemoryStream(imageBytes);
        using var image = Image.Load(inputStream);

        image.Mutate(x => x.BackgroundColor(Color.White));

        return CompressJpeg(image);
    }

    private static byte[] CompressJpeg(Image image)
    {
        for (int quality = 85; quality >= 10; quality -= 5)
        {
            using var outputStream = new MemoryStream();
            image.Save(outputStream, new JpegEncoder { Quality = quality });

            if (outputStream.Length <= TargetSizeBytes || quality <= 10)
                return outputStream.ToArray();
        }

        return CompressJpegWithResize(image);
    }

    private static byte[] CompressJpegWithResize(Image image)
    {
        double ratio = Math.Sqrt((double)TargetSizeBytes / MaxSizeBytes);

        image.Mutate(x => x.Resize(
            (int)(image.Width  * ratio),
            (int)(image.Height * ratio)));

        using var outputStream = new MemoryStream();
        image.Save(outputStream, new JpegEncoder { Quality = 75 });
        return outputStream.ToArray();
    }
}
