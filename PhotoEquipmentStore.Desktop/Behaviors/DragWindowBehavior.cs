using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Поведение перетаскивания окна за область <see cref="Border"/>.
/// Обрабатывает событие <see cref="InputElement.PointerPressedEvent"/>.
/// </summary>
public class DragWindowBehavior : Behavior<Border>
{
    /// <summary>
    /// Подписывается на нажатие указателя для начала перемещения окна.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not null)
            AssociatedObject.PointerPressed += OnPointerPressed;
    }

    /// <summary>
    /// Отписывается от обработчика нажатия указателя.
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject is not null)
            AssociatedObject.PointerPressed -= OnPointerPressed;
    }

    /// <summary>
    /// Запускает перемещение родительского окна при нажатии левой кнопки мыши.
    /// </summary>
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
        {
            var window = TopLevel.GetTopLevel(AssociatedObject) as Window;
            window?.BeginMoveDrag(e);
        }
    }
}
