using System;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace PhotoEquipmentStore.SettingsUI;

public class ThemeService
{
    public static void Toggle(bool isDark)
    {
        Avalonia.Application.Current!.RequestedThemeVariant =
            isDark ? ThemeVariant.Dark : ThemeVariant.Light;
    }
}
