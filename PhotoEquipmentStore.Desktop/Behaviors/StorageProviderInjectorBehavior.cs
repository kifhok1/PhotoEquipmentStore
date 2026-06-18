using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Поведение внедрения <see cref="IStorageProvider"/> в ViewModel при загрузке <see cref="UserControl"/>.
/// Обрабатывает событие <see cref="Control.Loaded"/>.
/// </summary>
public class StorageProviderInjectorBehavior : Behavior<UserControl>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.Loaded += OnLoaded;
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.Loaded -= OnLoaded;
        base.OnDetaching();
    }

    private void OnLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (AssociatedObject?.DataContext is IStorageProviderReceiver vm)
        {
            var topLevel = TopLevel.GetTopLevel(AssociatedObject);
            vm.StorageProvider = topLevel?.StorageProvider;
        }
    }
}
