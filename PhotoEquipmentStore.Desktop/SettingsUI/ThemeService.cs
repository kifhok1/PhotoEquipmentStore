using System;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace PhotoEquipmentStore.SettingsUI;/// <summary>
/// Переключение светлой и тёмной темы Avalonia.
/// </summary>


public class ThemeService
{
    /// <summary>
    /// Переключает светлую или тёмную тему приложения.
    /// </summary>
    public static void Toggle(bool isDark)
    {
        Avalonia.Application.Current!.RequestedThemeVariant =
            isDark ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
