using Avalonia.Controls;
using Avalonia.Media;
using Color = Avalonia.Media.Color;

namespace PhotoEquipmentStore.Helper;

public static/// <summary>
/// Получение цветов и кистей из ресурсов темы Avalonia.
/// </summary>
 class ColorProvider
{
    /// <summary>
    /// Возвращает цвет из ресурсов темы или запасной вариант.
    /// </summary>
    public static Color GetColor(string key, Color fallback)
    {
        if (Avalonia.Application.Current?.TryFindResource(key, out var res) == true
            && res is SolidColorBrush brush)
            return brush.Color;

        return fallback;
    }

    /// <summary>

    /// Возвращает кисть из ресурсов текущей темы.

    /// </summary>

    public static IBrush GetBrush(string key, Color fallback)
    {
        var app = Avalonia.Application.Current;
        if (app == null) return new SolidColorBrush(fallback);

        var theme = app.ActualThemeVariant;

        if (app.TryFindResource(key, theme, out var res) && res is IBrush brush)
            return brush;

        return new SolidColorBrush(fallback);
    }

}
