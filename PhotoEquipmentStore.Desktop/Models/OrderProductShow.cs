using Avalonia.Media.Imaging;
using PhotoEquipmentStore.Domain.Entities;
using ReactiveUI;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Модель товара на экране оформления заказа.
/// </summary>


public class OrderProductShow : ReactiveObject
{
    /// <summary>
    /// Уникальный идентификатор записи.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Наименование или ФИО.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Описание товара.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Цена товара.
    /// </summary>
    public int Price { get; set; }

    public int DiscountPercent { get; set; }

    /// <summary>

    /// Количество на складе или в позиции.

    /// </summary>

    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int ManufacturerId { get; set; }
    public string ManufacturerName { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; }
    public Bitmap? Image { get; set; }

    /// <summary>

    /// Признак наличия изображения у позиции.

    /// </summary>

    public bool HasImage => Image != null;

    public int DiscountAmount => Price * DiscountPercent / 100;

    /// <summary>

    /// Цена с учётом скидки.

    /// </summary>

    public int FinalPrice => Price - DiscountAmount;

    /// <summary>

    /// Размер скидки.

    /// </summary>

    public int Discount => DiscountAmount;

    private int _cartQuantity;
    /// <summary>
    /// Количество единиц в корзине.
    /// </summary>
    public int CartQuantity
    {
        get => _cartQuantity;
        set
        {
            this.RaiseAndSetIfChanged(ref _cartQuantity, value);
            this.RaisePropertyChanged(nameof(IsMaxReached));
        }
    }

    /// <summary>

    /// Достигнут ли лимит остатка на складе.

    /// </summary>

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
