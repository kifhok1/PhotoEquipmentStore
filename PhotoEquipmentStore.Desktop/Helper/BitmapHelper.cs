using System.IO;
using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Helper;/// <summary>
/// Вспомогательные методы конвертации между массивом байтов и <see cref="Bitmap"/>.
/// </summary>


public class BitmapHelper
{
    /// <summary>
    /// Создаёт <see cref="Bitmap"/> из массива байтов изображения.
    /// </summary>
    public static Bitmap? FromBytes(byte[]? bytes)
    {
        if (bytes is null || bytes.Length == 0)
            return null;

        using var ms = new MemoryStream(bytes);
        return new Bitmap(ms);
    }

    /// <summary>

    /// Сериализует <see cref="Bitmap"/> в массив байтов.

    /// </summary>

    public static byte[]? ToBytes(Bitmap? bitmap)
    {
        using var ms = new MemoryStream();
        bitmap.Save(ms);
        return ms.ToArray();
    }
}
