using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

/// <summary>
/// Утилита сжатия изображений перед сохранением в базу данных.
/// </summary>
public static class ImageCompressor
{
    private const long MaxSizeBytes    = 2 * 1024 * 1024;
    private const long TargetSizeBytes = 500 * 1024;

    /// <summary>
    /// Сжимает изображение, если его размер превышает допустимый порог.
    /// </summary>
    /// <param name="imageBytes">Исходные байты изображения.</param>
    /// <returns>Сжатые байты или исходные, если сжатие не требуется.</returns>
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

        // Заполняем прозрачные области белым фоном перед конвертацией в JPEG
        image.Mutate(x => x.BackgroundColor(Color.White));

        return CompressJpeg(image);
    }

    private static byte[] CompressJpeg(Image image)
    {
        // Постепенно снижаем качество JPEG, пока размер не достигнет целевого
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
        // Если снижение качества недостаточно — уменьшаем разрешение изображения
        double ratio = Math.Sqrt((double)TargetSizeBytes / MaxSizeBytes);

        image.Mutate(x => x.Resize(
            (int)(image.Width  * ratio),
            (int)(image.Height * ratio)));

        using var outputStream = new MemoryStream();
        image.Save(outputStream, new JpegEncoder { Quality = 75 });
        return outputStream.ToArray();
    }
}
