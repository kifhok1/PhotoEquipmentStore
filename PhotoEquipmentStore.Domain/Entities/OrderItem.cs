namespace PhotoEquipmentStore.Domain.Entities;

public class OrderItem
{
    private int productId;
    private string productName;
    private int quantity;
    private decimal price;
    private int discount;
    private byte[]? productImage;
    
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
    public int Quantity
    {
        get { return quantity; }
        set { quantity = value; }
    }
    public decimal Price
    {
        get { return price; }
        set { price = value; }
    }

    public int Discount
    {
        get { return discount; }
        set { discount = value; }
    }

    public byte[]? ProductImage
    {
        get { return productImage; }
        set { productImage = value; }
    }

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