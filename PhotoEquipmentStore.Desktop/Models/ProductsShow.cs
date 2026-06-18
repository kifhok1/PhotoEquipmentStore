using Avalonia.Media.Imaging;

namespace PhotoEquipmentStore.Models;/// <summary>
/// Модель товара для экрана управления каталогом.
/// </summary>


public class ProductsShow
{
    private int id;
    private string name;
    private string description;
    private int price;
    private int discount;
    private int quantity;
    private int categoryId;
    private string categoryName;
    private int manufacturerId;
    private string manufacturerName;
    private int supplierId;
    private string supplierName;
    private Bitmap image;

    /// <summary>

    /// Уникальный идентификатор записи.

    /// </summary>

    public int Id
    {
        get => id;
        set => id = value;
    }
    /// <summary>
    /// Наименование или ФИО.
    /// </summary>
    public string Name
    {
        get => name;
        set => name = value;
    }
    /// <summary>
    /// Цена товара.
    /// </summary>
    public int Price
    {
        get => price;
        set => price = value;
    }
    /// <summary>
    /// Размер скидки.
    /// </summary>
    public int Discount
    {
        get => discount;
        set => discount = value;
    }
    /// <summary>
    /// Количество на складе или в позиции.
    /// </summary>
    public int Quantity
    {
        get => quantity;
        set => quantity = value;
    }
    public int CategoryId
    {
        get => categoryId;
        set => categoryId = value;
    }
    public string CategoryName
    {
        get => categoryName;
        set => categoryName = value;
    }
    public int ManufacturerId
    {
        get => manufacturerId;
        set => manufacturerId = value;
    }

    public string ManufacturerName
    {
        get => manufacturerName;
        set => manufacturerName = value;
    }

    public int SupplierId
    {
        get => supplierId;
        set => supplierId = value;
    }

    public string SupplierName
    {
        get => supplierName;
        set => supplierName = value;
    }

    public Bitmap Image
    {
        get => image;
        set => image = value;
    }

    /// <summary>

    /// Описание товара.

    /// </summary>

    public string Description
    {
        get => description;
        set => description = value;
    }

    /// <summary>

    /// Признак наличия изображения.

    /// </summary>

    public bool IsVisibleImage
    {
        get => image != null;
    }

    public ProductsShow(
        int id,
        string name,
        int price,
        int discount,
        int quantity,
        int categoryId,
        string categoryName,
        int manufacturerId,
        string manufacturerName,
        int supplierId,
        string supplierName,
        string description,
        Bitmap image)
    {
        Id = id;
        Name = name;
        Price = price;
        Discount = discount;
        Quantity = quantity;
        CategoryId = categoryId;
        CategoryName = categoryName;
        ManufacturerId = manufacturerId;
        ManufacturerName = manufacturerName;
        SupplierId = supplierId;
        SupplierName = supplierName;
        Description = description;
        Image = image;
    }

    public ProductsShow(
        int id,
        string name,
        int price,
        int discount,
        int quantity,
        int categoryId,
        string categoryName,
        int manufacturerId,
        string manufacturerName,
        int supplierId,
        string supplierName,
        string description)
    {
        Id = id;
        Name = name;
        Price = price;
        Discount = discount;
        Quantity = quantity;
        CategoryId = categoryId;
        CategoryName = categoryName;
        ManufacturerId = manufacturerId;
        ManufacturerName = manufacturerName;
        SupplierId = supplierId;
        SupplierName = supplierName;
        Description = description;
    }
}
