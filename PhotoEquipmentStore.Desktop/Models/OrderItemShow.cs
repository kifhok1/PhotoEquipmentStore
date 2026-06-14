using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;

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

    public Bitmap? ProductImage
    {
        get { return productImage; }
        set { productImage = value; }
    }

    public decimal LineTotal => (Price - Price * Discount / 100m) * Quantity;
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
