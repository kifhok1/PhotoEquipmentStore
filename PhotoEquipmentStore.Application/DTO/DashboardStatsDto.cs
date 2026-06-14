namespace PhotoEquipmentStore.Application.DTO;

/// <summary>
/// Сводная статистика по заказам и выручке за день и месяц.
/// </summary>
public class DashboardStatsDto
{
    public int     OrdersToday  { get; }
    public int     ItemsToday   { get; }
    public decimal RevenueToday { get; }
    public int     OrdersMonth  { get; }
    public int     ItemsMonth   { get; }
    public decimal RevenueMonth { get; }

    public DashboardStatsDto(
        int     ordersToday,
        int     itemsToday,
        decimal revenueToday,
        int     ordersMonth,
        int     itemsMonth,
        decimal revenueMonth)
    {
        OrdersToday  = ordersToday;
        ItemsToday   = itemsToday;
        RevenueToday = revenueToday;
        OrdersMonth  = ordersMonth;
        ItemsMonth   = itemsMonth;
        RevenueMonth = revenueMonth;
    }

    public static DashboardStatsDto Empty =>
        new(0, 0, 0m, 0, 0, 0m);
}
