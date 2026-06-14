using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

public class DragWindowBehavior : Behavior<Border>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not null)
            AssociatedObject.PointerPressed += OnPointerPressed;
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject is not null)
            AssociatedObject.PointerPressed -= OnPointerPressed;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed)
        {
            var window = TopLevel.GetTopLevel(AssociatedObject) as Window;
            window?.BeginMoveDrag(e);
        }
    }
}