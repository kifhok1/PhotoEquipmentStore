using Avalonia.Xaml.Interactivity;
using PhotoEquipmentStore.ViewModels.Pages.Manager;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Поведение, инициализирующее диапазон дат отчёта при появлении экрана в визуальном дереве.
/// Обрабатывает событие <see cref="Behavior.OnAttachedToVisualTree"/>.
/// </summary>
public class InitializeDatesOnAttachedBehavior : Behavior<Avalonia.Controls.UserControl>
{
    /// <summary>
    /// Вызывает <see cref="ReportsViewModel.InitializeDates"/> при присоединении к визуальному дереву.
    /// </summary>
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject?.DataContext is ReportsViewModel vm)
            vm.InitializeDates();
    }
}
