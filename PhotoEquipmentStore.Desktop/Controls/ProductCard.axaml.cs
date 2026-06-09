using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

public partial class ProductCard : UserControl
{
    public static readonly StyledProperty<OrderProductShow?> ProductProperty =
        AvaloniaProperty.Register<ProductCard, OrderProductShow?>(nameof(Product));

    public static readonly StyledProperty<ReactiveCommand<OrderProductShow, Unit>> SelectCommandProperty =
        AvaloniaProperty.Register<ProductCard, ReactiveCommand<OrderProductShow, Unit>>(nameof(SelectCommand));

    public static readonly StyledProperty<ReactiveCommand<OrderProductShow, Unit>> AddToCartCommandProperty =
        AvaloniaProperty.Register<ProductCard, ReactiveCommand<OrderProductShow, Unit>>(nameof(AddToCartCommand));

    public OrderProductShow? Product
    {
        get => GetValue(ProductProperty);
        set => SetValue(ProductProperty, value);
    }

    public ReactiveCommand<OrderProductShow, Unit> SelectCommand
    {
        get => GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    public ReactiveCommand<OrderProductShow, Unit> AddToCartCommand
    {
        get => GetValue(AddToCartCommandProperty);
        set => SetValue(AddToCartCommandProperty, value);
    }

    public ProductCard()
    {
        InitializeComponent();

        if (Design.IsDesignMode)
        {
            Product = new OrderProductShow
            {
                Id = 1,
                Name = "Sony Alpha 7 IV Body",
                Price = 224820,
                Discount = 0,
                Quantity = 12,
                CategoryName = "Cameras",
                ManufacturerName = "Sony",
                SupplierName = "PhotoPro",
                Description = "Полнокадровая беззеркальная камера."
            };
        }
    }
}