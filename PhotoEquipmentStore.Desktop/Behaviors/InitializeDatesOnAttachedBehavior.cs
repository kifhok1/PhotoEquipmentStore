using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.ViewModels.Pages.Manager;

namespace PhotoEquipmentStore.Behaviors;

public class InitializeDatesOnAttachedBehavior : Behavior<Avalonia.Controls.UserControl>
{
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject?.DataContext is ReportsViewModel vm)
            vm.InitializeDates();
    }
}
