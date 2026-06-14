using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Domain.Entities;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;

public class OrderProductShow : ReactiveObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }

    public int DiscountPercent { get; set; }

    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int ManufacturerId { get; set; }
    public string ManufacturerName { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; }
    public Bitmap? Image { get; set; }

    public bool HasImage => Image != null;

    public int DiscountAmount => Price * DiscountPercent / 100;

    public int FinalPrice => Price - DiscountAmount;

    public int Discount => DiscountAmount;

    private int _cartQuantity;
    public int CartQuantity
    {
        get => _cartQuantity;
        set
        {
            this.RaiseAndSetIfChanged(ref _cartQuantity, value);
            this.RaisePropertyChanged(nameof(IsMaxReached));
        }
    }

    public bool IsMaxReached => CartQuantity >= Quantity;

    public OrderProductShow() { }

    public OrderProductShow(Product product, Bitmap? image = null)
    {
        Id              = product.Id;
        Name            = product.Name;
        Description     = product.Description;
        Price           = product.Price;
        DiscountPercent = product.Discount;
        Quantity        = product.Quantity;
        CategoryId      = product.CategoryId;
        CategoryName    = product.CategoryName;
        ManufacturerId  = product.ManufacturerId;
        ManufacturerName = product.ManufacturerName;
        SupplierId      = product.SupplierId;
        SupplierName    = product.SupplierName;
        Image           = image;
    }
}
