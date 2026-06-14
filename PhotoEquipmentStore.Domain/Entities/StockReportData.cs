
namespace PhotoEquipmentStore.Domain.Entities;

public class StockReportData
{
    public int    ProductId        { get; }
    public string ProductName      { get; }
    public string CategoryName     { get; }
    public string ManufacturerName { get; }
    public string SupplierName     { get; }
    public int    Quantity         { get; }

    public StockReportData(
        int    productId,
        string productName,
        string categoryName,
        string manufacturerName,
        string supplierName,
        int    quantity)
    {
        ProductId        = productId;
        ProductName      = productName;
        CategoryName     = categoryName;
        ManufacturerName = manufacturerName;
        SupplierName     = supplierName;
        Quantity         = quantity;
    }
}
