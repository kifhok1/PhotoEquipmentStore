using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace PhotoEquipmentStore.Behaviors;

/// <summary>
/// Поведение оформления рамки <see cref="ComboBox"/> при открытии и закрытии выпадающего списка.
/// Обрабатывает события <see cref="ComboBox.DropDownOpened"/> и <see cref="ComboBox.DropDownClosed"/>.
/// </summary>
public class ComboBoxDropDownBorderBehavior : Behavior<ComboBox>
{
    /// <summary>
    /// Подписывается на открытие и закрытие выпадающего списка.
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject!.DropDownOpened += OnOpened;
        AssociatedObject.DropDownClosed += OnClosed;
    }

    /// <summary>
    /// Отписывается от событий выпадающего списка.
    /// </summary>
    protected override void OnDetaching()
    {
        AssociatedObject!.DropDownOpened -= OnOpened;
        AssociatedObject.DropDownClosed -= OnClosed;
        base.OnDetaching();
    }

    /// <summary>
    /// Скругляет верхние углы и убирает нижнюю границу при открытии списка.
    /// </summary>
    private void OnOpened(object? sender, EventArgs e)
    {
        if (AssociatedObject is null) return;
        AssociatedObject.CornerRadius = new CornerRadius(6, 6, 0, 0);
        AssociatedObject.BorderThickness = new Thickness(1, 1, 1, 0);
    }

    /// <summary>
    /// Восстанавливает полную рамку при закрытии списка.
    /// </summary>
    private void OnClosed(object? sender, EventArgs e)
    {
        if (AssociatedObject is null) return;
        AssociatedObject.CornerRadius = new CornerRadius(6);
        AssociatedObject.BorderThickness = new Thickness(1);
    }
}
