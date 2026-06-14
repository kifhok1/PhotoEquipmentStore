namespace PhotoEquipmentStore.Domain.Entities;

public class DashboardStats
{
    public int     OrdersToday  { get; }
    public int     ItemsToday   { get; }
    public decimal RevenueToday { get; }
    public int     OrdersMonth  { get; }
    public int     ItemsMonth   { get; }
    public decimal RevenueMonth { get; }

    public DashboardStats(
        int ordersToday, int itemsToday, decimal revenueToday,
        int ordersMonth, int itemsMonth, decimal revenueMonth)
    {
        OrdersToday  = ordersToday;
        ItemsToday   = itemsToday;
        RevenueToday = revenueToday;
        OrdersMonth  = ordersMonth;
        ItemsMonth   = itemsMonth;
        RevenueMonth = revenueMonth;
    }
}
