using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;

public class OrderConfirmShow
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public int Price { get; set; }
    public int Discount { get; set; }
    public int Quantity { get; set; }
    public Bitmap? Image { get; set; }

    public bool HasImage => Image != null;
    public int FinalPrice => Discount > 0 ? Price - Discount : Price;
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