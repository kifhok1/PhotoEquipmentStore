using Avalonia;
using System;
using Avalonia.ReactiveUI;

namespace PhotoEquipmentStore;

/// <summary>
/// Точка входа в десктопное приложение Avalonia.
/// </summary>
sealed class Program
{

    /// <summary>
    /// Запускает приложение с классическим жизненным циклом рабочего стола.
    /// </summary>
    /// <param name="args">Аргументы командной строки.</param>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Создаёт и настраивает построитель Avalonia-приложения (ReactiveUI, шрифт, логирование).
    /// </summary>
    /// <returns>Настроенный экземпляр <see cref="AppBuilder"/>.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseReactiveUI()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
