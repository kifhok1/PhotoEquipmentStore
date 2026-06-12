namespace PhotoEquipmentStore.Domain.Entities;

public class PopularityReportData
{
    public int    Rank         { get; }
    public int    ProductId    { get; }
    public string ProductName  { get; }
    public string CategoryName { get; }
    public int    Price        { get; }
    public int    TotalSold    { get; }
    public int    OrdersCount  { get; }

    public PopularityReportData(
        int rank, int productId, string productName,
        string categoryName, int price, int totalSold, int ordersCount)
    {
        Rank         = rank;
        ProductId    = productId;
        ProductName  = productName;
        CategoryName = categoryName;
        Price        = price;
        TotalSold    = totalSold;
        OrdersCount  = ordersCount;
    }
}