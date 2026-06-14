using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

public class ComboBoxDropDownBorderBehavior : Behavior<ComboBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.DropDownOpened += OnOpened;
        AssociatedObject.DropDownClosed += OnClosed;
    }

    protected override void OnDetaching()
    {
        AssociatedObject!.DropDownOpened -= OnOpened;
        AssociatedObject.DropDownClosed -= OnClosed;
        base.OnDetaching();
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        if (AssociatedObject is null) return;
        AssociatedObject.CornerRadius = new CornerRadius(6, 6, 0, 0);
        AssociatedObject.BorderThickness = new Thickness(1, 1, 1, 0);
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        if (AssociatedObject is null) return;
        AssociatedObject.CornerRadius = new CornerRadius(6);
        AssociatedObject.BorderThickness = new Thickness(1);
    }
}
