using Avalonia.Media.Imaging;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Позиция товара в корзине заказа.
/// </summary>


public class OrderCartItem : ReactiveObject
{
    public int ProductId { get; set; }
    /// <summary>
    /// Наименование или ФИО.
    /// </summary>
    public string Name { get; set; }
    public string CategoryName { get; set; }
    /// <summary>
    /// Цена товара.
    /// </summary>
    public int Price { get; set; }
    /// <summary>
    /// Размер скидки.
    /// </summary>
    public int Discount { get; set; }
    public Bitmap? Image { get; set; }

    private int _cartQuantity;
    /// <summary>
    /// Количество единиц в корзине.
    /// </summary>
    public int CartQuantity
    {
        get => _cartQuantity;
        set => this.RaiseAndSetIfChanged(ref _cartQuantity, value);
    }

    /// <summary>

    /// Признак наличия изображения у позиции.

    /// </summary>

    public bool HasImage => Image != null;
    /// <summary>
    /// Цена с учётом скидки.
    /// </summary>
    public int FinalPrice => Discount > 0 ? Price - Discount : Price;
    /// <summary>
    /// Сумма строки (цена × количество).
    /// </summary>
    public int LineTotal => FinalPrice * CartQuantity;
    public int LineDiscount => Discount * CartQuantity;

    public OrderCartItem(OrderProductShow product)
    {
        ProductId    = product.Id;
        Name         = product.Name;
        CategoryName = product.CategoryName;
        Price        = product.Price;
        Discount     = product.Discount;
        CartQuantity = 1;
        Image        = product.Image;
    }
}
