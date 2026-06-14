using System.Reactive;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using PhotoEquipmentStore.Helper;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Пункт навигационного меню боковой панели.
/// </summary>


public class NavigationMenuItem : ReactiveObject
{
    private string title;
    private StreamGeometry icon;
    private ReactiveCommand<Unit, Unit> navigateCommand { get; init; } = null!;

    /// <summary>

    /// Заголовок или наименование записи.

    /// </summary>

    public string Title
    {
        get { return title; }
    }

    /// <summary>

    /// Векторная иконка пункта меню.

    /// </summary>

    public StreamGeometry Icon
    {
        get { return icon; }
    }

    /// <summary>

    /// Команда навигации по пункту меню.

    /// </summary>

    public ReactiveCommand<Unit, Unit> NavigateCommand
    {
        get { return navigateCommand; }
    }

    public NavigationMenuItem(string title, string icon,  ReactiveCommand<Unit, Unit> navigateCommand)
    {
        this.title = title;
        this.icon = IconProvider.Get(icon);
        this.navigateCommand = navigateCommand;
    }

    public NavigationMenuItem(string title, string icon)
    {
        this.title = title;
        this.icon = IconProvider.Get(icon);
    }
}
