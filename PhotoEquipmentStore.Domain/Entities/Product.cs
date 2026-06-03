namespace PhotoEquipmentStore.Domain.Entities;

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

    public int Id
    {
        get => id;
        set => id = value;
    }
    public string Name
    {
        get => name;
        set => name = value;
    }
    public int Price
    {
        get => price;
        set => price = value;
    }
    public int Discount
    {
        get => discount;
        set => discount = value;
    }
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

    public byte[]? Image
    {
        get => image;
        set => image = value;
    }

    public string Description
    {
        get => description;
        set => description = value;
    }
    
    public bool IsVisibleImage
    {
        get => image != null;
    }
    
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