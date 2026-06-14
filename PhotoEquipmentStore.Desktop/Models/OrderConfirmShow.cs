using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Позиция заказа на экране подтверждения.
/// </summary>


public class OrderConfirmShow
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
    /// <summary>
    /// Количество на складе или в позиции.
    /// </summary>
    public int Quantity { get; set; }
    public Bitmap? Image { get; set; }

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
    public int LineTotal => FinalPrice * Quantity;

    public string LinePriceLabel => Quantity > 1
        ? $"{Quantity} × {FinalPrice:N0} ₽"
        : $"{FinalPrice:N0} ₽";

    public static OrderConfirmShow FromCartItem(OrderCartItem item) => new()
    {
        ProductId    = item.ProductId,
        Name         = item.Name,
        CategoryName = item.CategoryName,
        Price        = item.Price,
        Discount     = item.Discount,
        Quantity     = item.CartQuantity,
        Image        = item.Image,
    };
}
