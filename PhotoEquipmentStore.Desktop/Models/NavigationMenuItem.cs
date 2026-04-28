using System.Reactive;
using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using PhotoEquipmentStore.Helper;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;

public class NavigationMenuItem : ReactiveObject
{
    private string title;
    private StreamGeometry icon;
    private ReactiveCommand<Unit, Unit> navigateCommand { get; init; } = null!;
    
    public string Title
    {
        get { return title; }
    }

    public StreamGeometry Icon
    {
        get { return icon; }
    }

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