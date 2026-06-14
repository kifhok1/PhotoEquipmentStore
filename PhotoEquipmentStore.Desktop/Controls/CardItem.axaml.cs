using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

public partial class CartItem : UserControl
{
    public static readonly StyledProperty<OrderCartItem?> ItemProperty =
        AvaloniaProperty.Register<CartItem, OrderCartItem?>(nameof(Item));

    public static readonly StyledProperty<ReactiveCommand<OrderCartItem, Unit>> IncreaseCommandProperty =
        AvaloniaProperty.Register<CartItem, ReactiveCommand<OrderCartItem, Unit>>(nameof(IncreaseCommand));

    public static readonly StyledProperty<ReactiveCommand<OrderCartItem, Unit>> DecreaseCommandProperty =
        AvaloniaProperty.Register<CartItem, ReactiveCommand<OrderCartItem, Unit>>(nameof(DecreaseCommand));

    public static readonly StyledProperty<ReactiveCommand<OrderCartItem, Unit>> RemoveCommandProperty =
        AvaloniaProperty.Register<CartItem, ReactiveCommand<OrderCartItem, Unit>>(nameof(RemoveCommand));

    public OrderCartItem? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    public ReactiveCommand<OrderCartItem, Unit> IncreaseCommand
    {
        get => GetValue(IncreaseCommandProperty);
        set => SetValue(IncreaseCommandProperty, value);
    }

    public ReactiveCommand<OrderCartItem, Unit> DecreaseCommand
    {
        get => GetValue(DecreaseCommandProperty);
        set => SetValue(DecreaseCommandProperty, value);
    }

    public ReactiveCommand<OrderCartItem, Unit> RemoveCommand
    {
        get => GetValue(RemoveCommandProperty);
        set => SetValue(RemoveCommandProperty, value);
    }

    public CartItem()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            Item = new OrderCartItem(new OrderProductShow
            {
                Id = 1,
                Name = "Sony Alpha 7 IV Body",
                Price = 224820,
                DiscountPercent = 0,
                Quantity = 12,
                CategoryName = "Cameras",
                ManufacturerName = "Sony",
                SupplierName = "PhotoPro",
                Description = "Полнокадровая беззеркальная камера."
            });
        }
    }
}
