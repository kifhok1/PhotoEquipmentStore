namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Товар фотооборудования с ценой, скидкой, остатком и привязкой к справочникам.
/// </summary>
public class Product
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
    private byte[]? image;

    /// <summary>
    /// Уникальный идентификатор товара.
    /// </summary>
    public int Id
    {
        get => id;
        set => id = value;
    }
    /// <summary>
    /// Наименование товара.
    /// </summary>
    public string Name
    {
        get => name;
        set => name = value;
    }
    /// <summary>
    /// Цена товара в рублях.
    /// </summary>
    public int Price
    {
        get => price;
        set => price = value;
    }
    /// <summary>
    /// Процент скидки на товар.
    /// </summary>
    public int Discount
    {
        get => discount;
        set => discount = value;
    }
    /// <summary>
    /// Количество товара на складе.
    /// </summary>
    public int Quantity
    {
        get => quantity;
        set => quantity = value;
    }
    /// <summary>
    /// Идентификатор категории товара.
    /// </summary>
    public int CategoryId
    {
        get => categoryId;
        set => categoryId = value;
    }
    /// <summary>
    /// Наименование категории товара.
    /// </summary>
    public string CategoryName
    {
        get => categoryName;
        set => categoryName = value;
    }
    /// <summary>
    /// Идентификатор производителя.
    /// </summary>
    public int ManufacturerId
    {
        get => manufacturerId;
        set => manufacturerId = value;
    }

    /// <summary>
    /// Наименование производителя.
    /// </summary>
    public string ManufacturerName
    {
        get => manufacturerName;
        set => manufacturerName = value;
    }

    /// <summary>
    /// Идентификатор поставщика.
    /// </summary>
    public int SupplierId
    {
        get => supplierId;
        set => supplierId = value;
    }

    /// <summary>
    /// Наименование поставщика.
    /// </summary>
    public string SupplierName
    {
        get => supplierName;
        set => supplierName = value;
    }

    /// <summary>
    /// Изображение товара в виде массива байтов.
    /// </summary>
    public byte[]? Image
    {
        get => image;
        set => image = value;
    }

    /// <summary>
    /// Текстовое описание товара.
    /// </summary>
    public string Description
    {
        get => description;
        set => description = value;
    }

    /// <summary>
    /// Признак наличия изображения у товара.
    /// </summary>
    public bool IsVisibleImage
    {
        get => image != null;
    }

    /// <summary>
    /// Создаёт товар с полным набором полей, включая изображение.
    /// </summary>
    public Product(
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
        byte[] image = null)
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

    /// <summary>
    /// Создаёт товар с полным набором полей без изображения.
    /// </summary>
    public Product(
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

    /// <summary>
    /// Создаёт товар с идентификаторами справочников без наименований.
    /// </summary>
    public Product(
        int id,
        string name,
        int price,
        int discount,
        int quantity,
        int categoryId,
        int manufacturerId,
        int supplierId,
        string description,
        byte[]? image = null)
    {
        Id = id;
        Name = name;
        Price = price;
        Discount = discount;
        Quantity = quantity;
        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        SupplierId = supplierId;
        Description = description;
        Image = image;
    }

    /// <summary>
    /// Создаёт новый товар без идентификатора для последующего сохранения в базу.
    /// </summary>
    public Product(
        string name,
        int price,
        int discount,
        int quantity,
        int categoryId,
        int manufacturerId,
        int supplierId,
        string description,
        byte[]? image = null)
    {
        Name = name;
        Price = price;
        Discount = discount;
        Quantity = quantity;
        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        SupplierId = supplierId;
        Description = description;
        Image = image;
    }
    /// <summary>
    /// Создаёт товар с идентификаторами справочников без изображения.
    /// </summary>
    public Product(
        int id,
        string name,
        int price,
        int discount,
        int quantity,
        int categoryId,
        int manufacturerId,
        int supplierId,
        string description)
    {
        Id = id;
        Name = name;
        Price = price;
        Discount = discount;
        Quantity = quantity;
        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        SupplierId = supplierId;
        Description = description;
    }

    /// <summary>
    /// Создаёт новый товар без идентификатора и изображения.
    /// </summary>
    public Product(
        string name,
        int price,
        int discount,
        int quantity,
        int categoryId,
        int manufacturerId,
        int supplierId,
        string description)
    {
        Name = name;
        Price = price;
        Discount = discount;
        Quantity = quantity;
        CategoryId = categoryId;
        ManufacturerId = manufacturerId;
        SupplierId = supplierId;
        Description = description;
    }
}
