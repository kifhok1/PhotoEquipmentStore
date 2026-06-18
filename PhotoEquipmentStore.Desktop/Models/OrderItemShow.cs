using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Позиция состава заказа.
/// </summary>


public class OrderItemShow
{
    private int productId;
    private string productName;
    private int quantity;
    private decimal price;
    private int discount;
    private Bitmap? productImage;

    public  int ProductId
    {
        get { return productId; }
        set { productId = value; }
    }
    public string ProductName
    {
        get { return productName; }
        set { productName = value; }
    }
    /// <summary>
    /// Количество на складе или в позиции.
    /// </summary>
    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }
    /// <summary>
    /// Цена товара.
    /// </summary>
    public decimal Price
    {
        get { return price; }
        set { price = value; }
    }

    /// <summary>

    /// Размер скидки.

    /// </summary>

    public int Discount
    {
        get { return discount; }
        set { discount = value; }
    }

    /// <summary>

    /// Изображение товара.

    /// </summary>

    public Bitmap? ProductImage
    {
        get { return productImage; }
        set { productImage = value; }
    }

    /// <summary>

    /// Сумма строки (цена × количество).

    /// </summary>

    public decimal LineTotal => (Price - Price * Discount / 100m) * Quantity;
    /// <summary>
    /// Признак наличия изображения у позиции.
    /// </summary>
    public bool HasImage => ProductImage != null;

    public OrderItemShow(
        int productId,
        string productName,
        int quantity,
        decimal price,
        int discount,
        Bitmap? productImage = null)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
        Discount = discount;
        ProductImage = productImage;
    }
}
