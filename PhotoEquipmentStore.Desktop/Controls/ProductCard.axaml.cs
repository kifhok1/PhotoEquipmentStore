using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using PhotoEquipmentStore.Models;
using ReactiveUI;

namespace PhotoEquipmentStore.Controls;

/// <summary>
/// Пользовательский элемент «карточка товара» для экрана оформления заказа.
/// Отображает данные <see cref="OrderProductShow"/> и пробрасывает команды выбора и добавления в корзину.
/// </summary>
public partial class ProductCard : UserControl
{
    /// <summary>
    /// Avalonia-свойство для привязки модели товара к разметке карточки.
    /// </summary>
    public static readonly StyledProperty<OrderProductShow?> ProductProperty =
        AvaloniaProperty.Register<ProductCard, OrderProductShow?>(nameof(Product));

    /// <summary>
    /// Avalonia-свойство команды, вызываемой при клике по всей карточке (выбор товара).
    /// </summary>
    public static readonly StyledProperty<ReactiveCommand<OrderProductShow, Unit>> SelectCommandProperty =
        AvaloniaProperty.Register<ProductCard, ReactiveCommand<OrderProductShow, Unit>>(nameof(SelectCommand));

    /// <summary>
    /// Avalonia-свойство команды добавления товара в корзину (кнопка «+»).
    /// </summary>
    public static readonly StyledProperty<ReactiveCommand<OrderProductShow, Unit>> AddToCartCommandProperty =
        AvaloniaProperty.Register<ProductCard, ReactiveCommand<OrderProductShow, Unit>>(nameof(AddToCartCommand));

    /// <summary>
    /// Модель товара, отображаемая в карточке (название, цена, изображение и т.д.).
    /// </summary>
    public OrderProductShow? Product
    {
        get => GetValue(ProductProperty);
        set => SetValue(ProductProperty, value);
    }

    /// <summary>
    /// Команда выбора товара; параметром передаётся текущий <see cref="Product"/>.
    /// </summary>
    public ReactiveCommand<OrderProductShow, Unit> SelectCommand
    {
        get => GetValue(SelectCommandProperty);
        set => SetValue(SelectCommandProperty, value);
    }

    /// <summary>
    /// Команда добавления товара в корзину; параметром передаётся текущий <see cref="Product"/>.
    /// </summary>
    public ReactiveCommand<OrderProductShow, Unit> AddToCartCommand
    {
        get => GetValue(AddToCartCommandProperty);
        set => SetValue(AddToCartCommandProperty, value);
    }

    /// <summary>
    /// Инициализирует компонент. В режиме дизайнера подставляет демо-данные для превью в IDE.
    /// </summary>
    public ProductCard()
    {
        InitializeComponent();

        // Демо-данные только для визуального редактора Avalonia (Design Mode)
        if (Design.IsDesignMode)
        {
            Product = new OrderProductShow
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
            };
        }
    }
}
