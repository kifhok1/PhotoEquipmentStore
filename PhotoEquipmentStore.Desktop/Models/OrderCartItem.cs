using Avalonia.Media.Imaging;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;

public class OrderCartItem : ReactiveObject
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string CategoryName { get; set; }
    public int Price { get; set; }
    public int Discount { get; set; }
    public Bitmap? Image { get; set; }

    private int _cartQuantity;
    public int CartQuantity
    {
        get => _cartQuantity;
        set => this.RaiseAndSetIfChanged(ref _cartQuantity, value);
    }

    public bool HasImage => Image != null;
    public int FinalPrice => Discount > 0 ? Price - Discount : Price;
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
