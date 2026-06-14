namespace PhotoEquipmentStore.Domain.Entities;

/// <summary>
/// Статистика для панели управления: количество заказов, товаров и выручка за текущий день и месяц.
/// </summary>
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
