using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using PhotoEquipmentStore.SettingsUI;
using PhotoEquipmentStore.ViewModels;
using PhotoEquipmentStore.Views;

namespace PhotoEquipmentStore;

/// <summary>
/// Главный класс Avalonia-приложения: загрузка XAML, тема и инициализация главного окна.
/// </summary>
public partial class App : Avalonia.Application
{
    /// <summary>
    /// Загружает ресурсы приложения и применяет сохранённую тему оформления.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ThemeService.Toggle(SettingsUIFileParser.GetTheme() == "Тёмная");
    }

    /// <summary>
    /// Создаёт главное окно с <see cref="MainViewModel"/> после инициализации фреймворка.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Отключает встроенную валидацию DataAnnotations, чтобы использовать собственные правила.
    /// </summary>
    private void DisableAvaloniaDataAnnotationValidation()
    {

        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}
