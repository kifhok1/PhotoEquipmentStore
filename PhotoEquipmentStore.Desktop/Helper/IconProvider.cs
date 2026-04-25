using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace PhotoEquipmentStore.Helper;

public static class IconProvider
{
    public static StreamGeometry Get(string key)
    {
        if (Application.Current!.Resources.TryGetResource(key, ThemeVariant.Default, out var resource)
            && resource is StreamGeometry geometry)
        {
            return geometry;
        }

        throw new KeyNotFoundException($"Иконка '{key}' не найдена в ресурсах");
    }
}