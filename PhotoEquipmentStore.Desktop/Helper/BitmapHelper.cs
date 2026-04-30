using System.IO;
using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Helper;

public class BitmapHelper
{
    public static Bitmap? FromBytes(byte[]? bytes)
    {
        if (bytes is null || bytes.Length == 0)
            return null;

        using var ms = new MemoryStream(bytes);
        return new Bitmap(ms);
    }
    
    public static byte[] ToBytes(Bitmap bitmap)
    {
        using var ms = new MemoryStream();
        bitmap.Save(ms);
        return ms.ToArray();
    }
}