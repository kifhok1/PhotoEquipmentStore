using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;

namespace PhotoEquipmentStore.Helper;

public static/// <summary>
/// Получение векторных иконок <see cref="StreamGeometry"/> из ресурсов приложения.
/// </summary>
 class IconProvider
{
    /// <summary>
    /// Возвращает иконку по ключу ресурса.
    /// </summary>
    public static StreamGeometry Get(string key)
    {
        if (Avalonia.Application.Current!.Resources.TryGetResource(key, ThemeVariant.Default, out var resource)
            && resource is StreamGeometry geometry)
        {
            return geometry;
        }

        throw new KeyNotFoundException($"Иконка '{key}' не найдена в ресурсах");
    }
}
