namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Позиция в составе заказа: товар, количество, цена и скидка.
/// </summary>
public class OrderItem
{
    private int productId;
    private string productName;
    private int quantity;
    private decimal price;
    private int discount;
    private byte[]? productImage;

    /// <summary>
    /// Идентификатор товара в позиции заказа.
    /// </summary>
    public  int ProductId
    {
        get { return productId; }
        set { productId = value; }
    }
    /// <summary>
    /// Наименование товара в позиции заказа.
    /// </summary>
    public string ProductName
    {
        get { return productName; }
        set { productName = value; }
    }
    /// <summary>
    /// Количество единиц товара в позиции.
    /// </summary>
    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }
    /// <summary>
    /// Цена единицы товара на момент оформления заказа.
    /// </summary>
    public decimal Price
    {
        get { return price; }
        set { price = value; }
    }

    /// <summary>
    /// Процент скидки, применённый к позиции.
    /// </summary>
    public int Discount
    {
        get { return discount; }
        set { discount = value; }
    }

    /// <summary>
    /// Изображение товара в позиции заказа.
    /// </summary>
    public byte[]? ProductImage
    {
        get { return productImage; }
        set { productImage = value; }
    }

    /// <summary>
    /// Создаёт позицию заказа с указанными параметрами.
    /// </summary>
    public OrderItem(
        int productId,
        string productName,
        int quantity,
        decimal price,
        int discount,
        byte[]? productImage = null)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        Price = price;
        Discount = discount;
        ProductImage = productImage;
    }
}
